// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines an exit command.
/// </summary>
public static class ExitCommand
{
    /// <summary>
    /// Registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to execute to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        var entry = new CommandRegistryEntry("exit");
        dispatcher.Register(entry, Execute);
    }

    private static CommandResult Execute(IShell shell, ICommandExecutor executor, CommandContext context)
    {
        shell.Quit();
        return CommandResult.Ok();
    }
}