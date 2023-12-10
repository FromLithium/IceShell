// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines a command that does nothing and always succeeds. This command is intended to be a comment command.
/// </summary>
public class RemarksCommand
{
    /// <summary>
    /// Constructs this command and registers it to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("rem", (_, _, _) => CommandResult.Ok())
            .ValueParameter("comment")
                .Position(0)
                .Build()
            .Build(dispatcher);
    }
}
