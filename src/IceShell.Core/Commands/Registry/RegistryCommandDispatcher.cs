// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using System.Diagnostics;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Parsing;
using IceShell.Core.Exceptions;
using IceShell.Parsing;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.CLI;
using NexusKrop.IceShell.Core.FileSystem;

internal class RegistryCommandDispatcher : IRegistryCommandDispatcher
{
    private readonly Dictionary<string, CommandRegistryEntry> _commandRegistry = new();
    private readonly Dictionary<string, string> _aliases = new();
    private readonly Dictionary<CommandRegistryEntry, CommandDelegate> _commandDelegates = new();

    public int AliasCount => _aliases.Count;

    public IEnumerable<string> EnumerateAliases()
    {
        return _aliases.Keys;
    }

    public IEnumerable<string> EnumerateAliases(string commandId)
    {
        if (!_commandRegistry.ContainsKey(commandId))
        {
            return Enumerable.Empty<string>();
        }

        return _aliases.Where(x => x.Value == commandId).Select(x => x.Key);
    }

    public bool AnyCommand()
    {
        return _commandRegistry.Count > 0;
    }

    public void Register(CommandRegistryEntry command, CommandDelegate routine)
    {
        _commandRegistry[command.Identifier] = command;
        _commandDelegates[command] = routine;
        PutAlias(command.Identifier, command.Identifier);
    }

    public bool SearchInfoByAlias(string alias, out CommandRegistryEntry? result)
    {
        if (!_aliases.TryGetValue(alias, out var id))
        {
            result = null;
            return false;
        }

        if (!_commandRegistry.TryGetValue(id, out var entry))
        {
            result = null;
            return false;
        }

        result = entry;
        return true;
    }

    public bool HasAlias(string alias)
    {
        return _commandRegistry.ContainsKey(alias.ToUpperInvariant());
    }

    public void PutAlias(string alias, string id)
    {
        if (!_commandRegistry.ContainsKey(id))
        {
            throw new ArgumentException("The specified ID does not exist.", nameof(id));
        }
        
        _aliases.Add(alias, id);
    }

    public CommandSectionCompound Parse(string line)
    {
        var statements = new LineParser().ParseLine(line);

        var rawCompound = CommandParser.ParseCompound(statements, HasAlias);
        var sysCompound = rawCompound.Select(ParseSection).ToList();

        return new CommandSectionCompound(sysCompound);
    }

    public CommandSection ParseSection(SyntaxSegment segment)
    {
        if (segment.Type == SegmentType.ExternalCommand)
        {
            return new CommandSection(segment.ExternalCommand);
        }

        if (segment.InternalCommand == null)
        {
            throw new InvalidOperationException("The segment is invalid.");
        }
                
        if (!_commandRegistry.TryGetValue(segment.InternalCommand.Name, out var definition))
        {
            throw new ArgumentException("The specified command is not associated with this dispatcher.",
                nameof(segment));
        }

        var arguments = InternalParseArgument(segment.InternalCommand, definition);

        var cmdInfo = new RegistryCommandParseResult(definition.Identifier, arguments);

        return new CommandSection(cmdInfo, segment.InternalCommand!.Name, segment.NextAction);
    }

    private static CommandArgumentResult InternalParseArgument(SyntaxCommandInvocation invocation, CommandRegistryEntry entry)
    {
        var parsingProvider = new CommandParsingProvider(invocation, entry);
        return parsingProvider.Parse();
    }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, string line)
    {
        return Execute(shell, executor, Parse(line));
    }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context, RegistryCommandParseResult parseResult)
    {
        if (!_commandRegistry.TryGetValue(parseResult.CommandId, out var command) ||
            !_commandDelegates.TryGetValue(command, out var routine))
        {
            throw new ArgumentException("The specified command is not associated with this dispatcher.",
                nameof(parseResult));
        }

        try
        {
            return routine(shell, executor, new CommandContext(parseResult.Arguments, context.NextAction, context.Retrieval));
        }
        catch (CommandInterruptException e)
        {
            return e.Result;
        }
    }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context, CommandSection section)
    {
        if (string.IsNullOrWhiteSpace(section.Name))
        {
            // This indicates a blank line which we will just skip.
            return CommandResult.Ok();
        }

        if (section.IsCommand && section.Command != null)
        {
            var cmd = section.Command.Value;
            
            return Execute(shell, executor, context, cmd);
        }
        else if (section.Statements != null)
        {
            var args = section.Statements.Select(x => x.Content);

            var enumerable = args.ToList();
            if (!enumerable.Any())
            {
                // This also indicates a blank line.
                return CommandResult.Ok();
            }

            var cmdName = section.Name;
            var searchResult = PathSearcher.GetSystemExecutableName(Path.Combine(Environment.CurrentDirectory, cmdName));

            if (searchResult == null || !File.Exists(searchResult))
            {
                throw new CommandFormatException(LangMessage.MsgUnknownCommand(cmdName));
            }

            var processInfo = new ProcessStartInfo(searchResult);
            enumerable.ForEach(processInfo.ArgumentList.Add);
            processInfo.UseShellExecute = false;
            processInfo.WorkingDirectory = Environment.CurrentDirectory;

            var process = Process.Start(processInfo);

            if (process == null)
            {
                ConsoleOutput.PrintShellError(LangMessage.MsgUnableStartProcess());
                return CommandResult.WithError(CommandErrorCode.ExternalStartFail);
            }

            process.WaitForExit();

            return CommandResult.WithCode(process.ExitCode);
        }
        else
        {
            throw new ArgumentException("Invalid batch line", nameof(section));
        }
    }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, CommandSectionCompound compound)
    {
        var inAction = false;

        // This nextAction variable is the NextAction of the *previous* command.
        // For the NextAction for the current command, use context.NextAction.
        var nextAction = SyntaxNextAction.None;

        // The redirected standard output of the last command.
        TextReader? lastOutStream = null;

        foreach (var line in compound)
        {
            var context = new ExecutionContext(null, line.NextAction);

            if (nextAction == SyntaxNextAction.Redirect)
            {
                context.Retrieval = lastOutStream;
                lastOutStream = null;
            }

            var exitCode = CommandResult.WithError(CommandErrorCode.ExternalStartFail);
            TextReader? pipeStream = null;

            if (line.IsCommand)
            {
                exitCode = Execute(shell, executor, context, line);

                if (exitCode.OutStream != null)
                {
                    pipeStream = exitCode.OutStream;
                }
            }
            else
            {
                var startInfo = Executive.CreateStartInfo(line);

                // RedirectTo: Redirect from last section to this section.
                var redirectTo = inAction && nextAction == SyntaxNextAction.Redirect && context.Retrieval != null;

                // RedirectFrom: Redirect from this section to next section.
                var redirectFrom = context.NextAction == SyntaxNextAction.Redirect;

                startInfo.RedirectStandardOutput = redirectFrom;
                startInfo.RedirectStandardInput = redirectTo;

                var process = Process.Start(startInfo)
                    ?? throw new InvalidOperationException("Failed to create process.");

                // The redirection of the standard input from last command.
                if (redirectTo)
                {
                    process.StandardInput.Write(context.Retrieval!.ReadToEnd());
                    context.Retrieval.Dispose();
                }

                Executive.WatchProcess(process);
                process.WaitForExit();

                // The redirection of the standard output from this command.
                if (redirectFrom)
                {
                    pipeStream = process.StandardOutput;
                }
            }

            if (exitCode.ExitCode != 0)
            {
                // Command failure
                return exitCode;
            }

            lastOutStream = pipeStream;
            nextAction = line.NextAction;
            inAction = nextAction != SyntaxNextAction.None;
        }

        if (inAction)
        {
            throw new CommandFormatException(LangMessage.MsgActionNeverComplete());
        }

        if (lastOutStream != null)
        {
            Console.WriteLine(lastOutStream.ReadToEnd());
        }

        return CommandResult.Ok();
    }
}