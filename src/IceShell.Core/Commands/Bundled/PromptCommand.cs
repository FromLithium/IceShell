// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines a command that changes the IceShell command prompt.
/// </summary>
public static class PromptCommand
{
    private const string MsgInvalidTemplate = "command.prompt.invalid_template";
    private const string ValueParameterPrompt = "prompt";
    
    /// <summary>
    /// The ID of this command.
    /// </summary>
    private const string CommandId = "prompt";
    
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder(CommandId, Routine)
            .ValueParameter(ValueParameterPrompt, 0)
            .GreedyString()
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var prompt = context.GetValue(ValueParameterPrompt);

        if (string.IsNullOrWhiteSpace(prompt))
        {
            // Reset prompt.

            shell.Prompt = Shell.DefaultPrompt;
            return CommandResult.Ok();
        }

        // Validate the prompt.
        if (!Executive.TryParsePrompt(prompt, out _))
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument,
                LangMessage.Get(MsgInvalidTemplate));
        }

        shell.Prompt = prompt;
        return CommandResult.Ok();
    }
}
