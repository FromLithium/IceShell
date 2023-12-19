// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Commands.Utilities;

/// <summary>
/// Defines a command which copies one or more commands.
/// </summary>
public static class CopyCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("copy", Routine)
            .ConfigureDestinationCommand()
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandDocument executor, CommandContext context)
    {
        var fileContext = new DestinationFileCommandContext(context);

        return fileContext.Do(File.Copy);
    }
}