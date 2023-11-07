// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Batching.Commands;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;

[CommandInfo("goto")]
public class GotoCommand : IShellCommand
{
    [Value("label", true, 0)]
    public string? Label { get; set; }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (!executor.SupportsJump)
        {
            return CommandResult.WithError(CommandErrorCode.OperationNotSupported);
        }

        executor.Jump(Label!);
        return CommandResult.Ok();
    }
}
