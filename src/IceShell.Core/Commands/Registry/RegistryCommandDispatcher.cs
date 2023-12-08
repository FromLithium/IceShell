// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

// ----------------------------------------------------------------------------
//
// !!! READ BEFORE EDITING THIS FILE !!!
// Some important implementation details API contract about this class:
//
// 1. Command aliases are case-insensitive. When checking or registering new
//    aliases, be sure to convert them to their upper-case equivalent by
//    using ToUpperInvariant().
//
// 2. Command IDs, in other hand, are case-sensitive.
//
// If any of these are broken then features broken these rules cannot work.
//
// ----------------------------------------------------------------------------

namespace IceShell.Core.Commands.Registry;

using System.Diagnostics;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Parsing;
using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using IceShell.Core.Syntax;
using IceShell.Parsing;
using NexusKrop.IceShell.Core.CLI;
using NexusKrop.IceShell.Core.Commands.Bundled;

/// <summary>
/// Provides the default implementation for the <see cref="IRegistryCommandDispatcher"/> interface. Do not use the API directly;
/// rather, store and pass this instance as <see cref="IRegistryCommandDispatcher"/>.
/// </summary>
public class RegistryCommandDispatcher : IRegistryCommandDispatcher
{
    private readonly Dictionary<string, CommandRegistryEntry> _commandRegistry = new();
    private readonly Dictionary<string, string> _aliases = new();
    private readonly Dictionary<CommandRegistryEntry, CommandDelegate> _commandDelegates = new();
    private readonly bool _parseArgumentVariables;
    
    internal RegistryCommandDispatcher(bool parseArgumentVariables = false)
    {
        _parseArgumentVariables = parseArgumentVariables;
    }
    
    /// <inheritdoc />
    public int AliasCount => _aliases.Count;

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases()
    {
        return _aliases.Keys;
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases(string commandId)
    {
        if (!_commandRegistry.ContainsKey(commandId))
        {
            return Enumerable.Empty<string>();
        }

        return _aliases.Where(x => x.Value == commandId).Select(x => x.Key);
    }

    /// <inheritdoc />
    public bool AnyCommand()
    {
        return _commandRegistry.Count > 0;
    }

    /// <inheritdoc />
    public bool AnyCommand(string id)
    {
        return _commandRegistry.ContainsKey(id);
    }

    /// <inheritdoc />
    public void Register(CommandRegistryEntry command, CommandDelegate routine)
    {
        _commandRegistry[command.Identifier] = command;
        _commandDelegates[command] = routine;
        PutAlias(command.Identifier, command.Identifier);
    }

    /// <inheritdoc />
    public bool SearchInfoByAlias(string alias, out CommandRegistryEntry? result)
    {
        if (!_aliases.TryGetValue(alias.ToUpperInvariant(), out var id))
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

    /// <inheritdoc />
    public bool HasAlias(string alias)
    {
        return _aliases.ContainsKey(alias.ToUpperInvariant());
    }

    /// <inheritdoc />
    public void PutAlias(string alias, string id)
    {
        if (!_commandRegistry.ContainsKey(id))
        {
            throw new ArgumentException("The specified ID does not exist.", nameof(id));
        }
        
        _aliases.Add(alias.ToUpperInvariant(), id);
    }

    /// <inheritdoc />
    public CommandSectionCompound Parse(ReadOnlySpan<char> line)
    {
        var statements = new CommandWordBreaker().RunParse(line);

        var rawCompound = CommandParser.ParseCompound(statements, HasAlias, x => PathSearcher.ExpandHome(x));
        var sysCompound = rawCompound.Select(ParseSection);

        return new CommandSectionCompound(sysCompound);
    }

    /// <inheritdoc />
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

        if (!_aliases.TryGetValue(segment.InternalCommand.Name.ToUpperInvariant(),
            out var id))
        {
            throw new ArgumentException("The specified command is not associated with this dispatcher.",
                nameof(segment));
        }
        
        if (!_commandRegistry.TryGetValue(id, out var definition))
        {
            throw new ArgumentException("The specified command that the aliases is pointing to is not associated with this dispatcher.",
                nameof(segment));
        }

        var arguments = InternalParseArgument(segment.InternalCommand, definition);

        var cmdInfo = new RegistryCommandParseResult(definition.Identifier, arguments);

        return new CommandSection(cmdInfo, segment.InternalCommand!.Name, segment.NextAction);
    }

    private CommandArgumentResult InternalParseArgument(SyntaxCommandInvocation invocation, CommandRegistryEntry entry)
    {
        var parsingProvider = new CommandParsingProvider(invocation, entry, _parseArgumentVariables);
        
        // TODO optimise this by separating all value options with switches.
        return parsingProvider.Parse(entry.Options.Where(x => x.Value.IsSwitch)
            .Select(x => x.Key)
            .ToHashSet());
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, string line)
    {
        return Execute(shell, document, Parse(line));
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, ExecutionContext context, RegistryCommandParseResult parseResult)
    {
        if (!_commandRegistry.TryGetValue(parseResult.CommandId, out var command) ||
            !_commandDelegates.TryGetValue(command, out var routine))
        {
            throw new ArgumentException("The specified command is not associated with this dispatcher.",
                nameof(parseResult));
        }

        if (parseResult.Arguments.IsHelpInformationRequested)
        {
            HelpCommandEx.PrintHelp(command, this);
            return CommandResult.Ok();
        }

        try
        {
            return routine(shell, document, new CommandContext(parseResult.Arguments, context.NextAction, context.Retrieval));
        }
        catch (CommandInterruptException e)
        {
            return e.Result;
        }
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, ExecutionContext context, CommandSection section)
    {
        if (string.IsNullOrWhiteSpace(section.Name))
        {
            // This indicates a blank line which we will just skip.
            return CommandResult.Ok();
        }

        if (section is { IsCommand: true, Command: not null })
        {
            var cmd = section.Command.Value;
            
            return Execute(shell, document, context, cmd);
        }
        else if (section.Statements != null)
        {
            // Execute external command.
            
            var args = section.Statements.Select(x => x.Content);

            var argList = args.ToList();
            if (argList.Count == 0)
            {
                // This also indicates a blank line.
                return CommandResult.Ok();
            }

            var cmdName = section.Name;
            var searchResult = PathSearcher.GetSystemExecutableName(Path.Combine(Environment.CurrentDirectory, cmdName));

            // If unable to find file...
            if (searchResult == null || !File.Exists(searchResult))
            {
                throw new CommandFormatException(LangMessage.MsgUnknownCommand(cmdName));
            }

            var processInfo = new ProcessStartInfo(searchResult);
            
            argList.ForEach(x =>
            {
                // If this dispatcher is configured to expand variables,
                // then expand variables.
                if (_parseArgumentVariables)
                {
                    x = PathSearcher.ExpandVariables(x, true);
                }
                
                processInfo.ArgumentList.Add(x);
            });
            
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

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, CommandSectionCompound compound)
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
                
                // Justification: this is a fail-safe measure.
                // ReSharper disable once RedundantAssignment
                lastOutStream = null;
            }

            CommandResult exitCode;
            TextReader? pipeStream = null;

            if (line.IsCommand)
            {
                exitCode = Execute(shell, document, context, line);

                if (exitCode.OutStream != null)
                {
                    pipeStream = exitCode.OutStream;
                }
            }
            else
            {
                // If no such external command is found, then we can declare that the command does not exist
                // and we should report bad command rather than throwing an exception.
                if (!Executive.TryCreateStartInfo(line, out var startInfo, out var isExecutable)
                    || startInfo == null)
                {
                    return CommandResult.WithError(CommandErrorCode.BadCommand,
                        LangMessage.GetFormat("shell.bad_command", line.Name));
                }

                // RedirectTo: Redirect from last section to this section.
                var redirectTo = inAction && nextAction == SyntaxNextAction.Redirect && context.Retrieval != null;

                // RedirectFrom: Redirect from this section to next section.
                var redirectFrom = context.NextAction == SyntaxNextAction.Redirect;

                startInfo.RedirectStandardOutput = redirectFrom;
                startInfo.RedirectStandardInput = redirectTo;

                var process = Process.Start(startInfo);

                // No process is started thus the startup is failed.
                if (process == null)
                {
                    return CommandResult.WithError(CommandErrorCode.ExternalStartFail);
                }

                // The redirection of the standard input from last command.
                if (redirectTo)
                {
                    if (!isExecutable)
                    {
                        return CommandResult.WithError(CommandErrorCode.PipelineToNotSupported);
                    }
                    
                    process.StandardInput.Write(context.Retrieval!.ReadToEnd());
                    context.Retrieval.Dispose();
                }

                Executive.WatchProcess(process);

                if (isExecutable)
                {
                    process.WaitForExit();
                }

                // If it is not executable, we start the app, its running, and we are OK.
                // If it is, because we waited for its exit, we get its exit code.
                exitCode = isExecutable ?
                    CommandResult.WithCode(process.ExitCode) :
                    CommandResult.Ok();

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