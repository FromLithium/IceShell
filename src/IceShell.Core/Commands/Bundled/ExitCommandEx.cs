// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using NexusKrop.IceShell.Core.Commands.Complex;

/// <summary>
/// Quits the IceShell program.
/// </summary>
[CommandInfo("exit")]
public class ExitCommandEx : IShellCommand
{
    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        shell.Quit();
        return CommandResult.Ok();
    }
}
