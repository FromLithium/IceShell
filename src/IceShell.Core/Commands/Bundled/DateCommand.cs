// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Attributes;

/// <summary>
/// A command that displays the current date and time.
/// </summary>
[CommandInfo("date")]
public class DateCommand : IShellCommand
{
    /// <summary>
    /// Gets or sets a value indicating whether the command shall only output the date itself.
    /// </summary>
    [Option('B', false)]
    public bool Bare { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command shall output in ISO 8601 format.
    /// </summary>
    [Option('O', false)]
    public bool UseIsoFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command shall output in RFC 1123 format.
    /// </summary>
    [Option('R', false)]
    public bool UseRfcFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command shall output universal time.
    /// </summary>
    [Option('U', false)]
    public bool UniversalTime { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var now = UniversalTime ? DateTime.UtcNow : DateTime.Now;
        string? dateStr = null;
        string? fullStr = null;

        // One cannot use RFC time and ISO time together at the same time.
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (UseIsoFormat && UseRfcFormat)
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.GetFormat("command.date.error.no_rfc_and_iso"));
        }

        // Check which format to use.
        if (UseIsoFormat)
        {
            dateStr = now.ToString("O");
        }
        else if (UseRfcFormat)
        {
            dateStr = now.ToString("R");
        }
        else
        {
            dateStr = now.ToString();
        }

        fullStr = Bare ? dateStr :LangMessage.GetFormat("command.date.current_is", dateStr);
        return CommandResult.Ok(new StringReader(fullStr));
    }
}