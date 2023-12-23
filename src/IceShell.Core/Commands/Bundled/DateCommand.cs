// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines a command that displays the current date and time.
/// </summary>
public static class DateCommand
{
    private const char SwitchBareMode = 'B';
    private const char SwitchUseIsoFormat = 'O';
    private const char SwitchUseRfcFormat = 'R';
    private const char SwitchUseUniversalTime = 'U';

    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("date", Routine)
            .SwitchOption(SwitchBareMode)
            .SwitchOption(SwitchUseIsoFormat)
            .SwitchOption(SwitchUseRfcFormat)
            .SwitchOption(SwitchUseUniversalTime)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var useUniversalTime = context.GetSwitch(SwitchUseUniversalTime);
        var useIsoFormat = context.GetSwitch(SwitchUseIsoFormat);
        var useRfcFormat = context.GetSwitch(SwitchUseRfcFormat);
        var bareMode = context.GetSwitch(SwitchBareMode);
        
        var now = useUniversalTime ? DateTime.UtcNow : DateTime.Now;
        string? dateStr = null;
        string? fullStr = null;

        // One cannot use RFC time and ISO time together at the same time.
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (useIsoFormat && useRfcFormat)
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.GetFormat("command.date.error.no_rfc_and_iso"));
        }

        // Check which format to use.
        if (useIsoFormat)
        {
            dateStr = now.ToString("O");
        }
        else if (useRfcFormat)
        {
            dateStr = now.ToString("R");
        }
        else
        {
            dateStr = now.ToString();
        }

        fullStr = bareMode ? dateStr :LangMessage.GetFormat("command.date.current_is", dateStr);
        return CommandResult.Ok(new StringReader(fullStr));
    }
}