// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using NexusKrop.IceShell.Core.Commands.Complex;
using System;

/// <summary>
/// Clears the screen.
/// </summary>
/// <seealso cref="Console.Clear()"/>
[CommandInfo("cls")]
public class ClearScreenCommandEx : IShellCommand
{
    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        Console.Clear();
        return CommandResult.Ok();
    }
}
