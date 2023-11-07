// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;

/// <summary>
/// Defines a command that does nothing and always succeeds. This command is intended to be a comment command.
/// </summary>
[CommandInfo("rem")]
[GreedyString]
public class RemarksCommand : IShellCommand
{
    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    [Value("comment", position: 0)]
    public string? Comment { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        return CommandResult.Ok();
    }
}
