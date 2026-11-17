// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Parsing;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Commands.Utilities;
using IceShell.Core.Exceptions;
using IceShell.Parsing;
using NexusKrop.IceShell.Core.CLI;

/// <summary>
/// Provides compatibility helpers for commands written for the registry command system.
/// </summary>
public static class CommandCompat
{
    /// <summary>
    /// Wraps a registry command so that it can be used as an internal command of the simple
    /// command system.
    /// </summary>
    /// <param name="entry">The registry entry that describes the command.</param>
    /// <param name="delegate">The command routine.</param>
    /// <returns>An internal command that invokes the specified registry command.</returns>
    public static InternalCommand FromRegistry(CommandRegistryEntry entry, CommandDelegate @delegate)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(@delegate);

        return (shell, document, args) => ExecuteRegistryCommand(entry,
            @delegate,
            shell,
            document,
            args);
    }

    private static int ExecuteRegistryCommand(CommandRegistryEntry entry,
        CommandDelegate @delegate,
        IShell shell,
        ICommandDocument document,
        IReadOnlyList<SyntaxWord> args)
    {
        try
        {
            var invocation = CommandParser.ParseSingleCommand(CreateInvocationWords(entry, args));
            var provider = new CommandParsingProvider(invocation, entry, document);
            var switches = entry.Options.Where(x => x.Value.IsSwitch)
                .Select(x => x.Key)
                .ToHashSet();
            var parsed = provider.Parse(switches);

            if (parsed.IsHelpInformationRequested)
            {
                HelpCommand.PrintHelp(entry);
                return 0;
            }

            var result = @delegate(shell,
                document,
                new CommandContext(parsed, SyntaxNextAction.None));

            // Process the output stream, copy its contents into the standard output.
            if (result.OutStream != null)
            {
                try
                {
                    StreamHelper.CopyText(result.OutStream, Console.Out);
                }
                catch (IOException ex)
                {
                    ConsoleOutput.PrintShellError(LangMessage.Get(
                        "generic_out_stream_read_error"));
                    ConsoleOutput.PrintShellError(ex.ToString());

                    return result.ExitCode == 0
                        ? 1
                        : result.ExitCode;
                }
                finally
                {
                    result.OutStream.Dispose();
                }
            }

            return ToExitCode(result);
        }
        catch (CommandInterruptException ex)
        {
            return ToExitCode(ex.Result);
        }
        catch (CommandFormatException ex)
        {
            PrintError(ex.Message);
            return -2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return -1;
        }
    }

    private static List<SyntaxWord> CreateInvocationWords(CommandRegistryEntry entry,
        IReadOnlyList<SyntaxWord> args)
    {
        var result = new List<SyntaxWord>(args.Count + 1)
        {
            new(entry.Identifier)
        };

        foreach (SyntaxWord arg in args)
        {
            result.Add(arg);
        }

        return result;
    }

    private static int ToExitCode(CommandResult result)
    {
        if (result.ErrorCode == CommandErrorCode.None)
        {
            return result.ExitCode;
        }

        var message = !string.IsNullOrWhiteSpace(result.Message)
            ? result.Message
            : LangMessage.MsgFromErrorCode(result.ErrorCode);

        PrintError(message);

        return result.ExitCode == 0 ? -2 : result.ExitCode;
    }

    private static void PrintError(string message)
    {
        Console.Error.WriteLine(message);
    }
}
