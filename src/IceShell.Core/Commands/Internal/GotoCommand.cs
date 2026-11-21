using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

namespace IceShell.Core.Commands.Internal;

internal sealed class GotoCommand
{
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("goto", Execute)
            .ValueParameter("batch")
                .Position(0)
                .Required()
                .NoExpandVariables()
                .Build()
            .Build(dispatcher);
    }

    private static CommandResult Execute(IShell shell,
        ICommandDocument document,
        CommandContext context)
    {
        if (!document.IsJumpSupported)
        {
            return CommandResult.WithError(CommandErrorCode.OperationNotSupported);
        }

        document.Jump(context.GetValue("batch")!);
        return CommandResult.Ok();
    }
}
