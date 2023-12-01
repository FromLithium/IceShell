// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that displays output.
/// </summary>
public static class EchoCommand
{
    /// <summary>
    /// Registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("echo", Routine)
            .ValueParameter("message", 0, required: false)
            .SwitchOption('P')
            .GreedyString()
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandExecutor executor, CommandContext context)
    {
        var message = context.GetValue("message");
        var expandLikePath = context.GetSwitch('P');
        
        if (context.Retrieval == null)
        {
            // If the message is empty, add an empty line.
            if (message == null)
            {
                Console.WriteLine();
                return CommandResult.Ok();
            }
            
            var finalText = message;

            // Expand variables and home directory.
            if (expandLikePath)
            {
                finalText = PathSearcher.ExpandVariables(message, true, expandLikePath);
            }

            Console.WriteLine(finalText);
        }
        else
        {
            Console.WriteLine(context.Retrieval.ReadToEnd());
        }

        return CommandResult.Ok();
    }
}