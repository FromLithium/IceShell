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
            .ValueParameter("message", 0, required: true)
            .SwitchOption('E')
            .SwitchOption('P')
            .GreedyString()
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandExecutor executor, CommandContext context)
    {
        var message = context.GetValue("message");
        var expandVariables = context.GetSwitch('E');
        var expandLikePath = context.GetSwitch('P');
        
        if (context.Retrieval == null)
        {
            var finalText = message;

            if ((expandVariables || expandLikePath) && message != null)
            {
                finalText = PathSearcher.ExpandVariables(message, true, expandLikePath);
            }

            Console.WriteLine(finalText ?? Environment.NewLine);
        }
        else
        {
            Console.WriteLine(context.Retrieval.ReadToEnd());
        }

        return CommandResult.Ok();
    }
}