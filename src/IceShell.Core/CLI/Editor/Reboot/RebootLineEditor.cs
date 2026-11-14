namespace IceShell.Core.CLI.Editor.Reboot;

using IceShell.Core.Api.Commands;
using ReadLineReboot;

/// <summary>
/// Provides an implementation of line editing functionality based on the managed library
/// <c>ReadLineReboot</c>.
/// </summary>
public sealed class RebootLineEditor : ILineEditor
{
    /// <inheritdoc/>
    public string? DoReadLine(IRegistryCommandDispatcher dispatcher, string? prompt)
    {
        return ReadLine.Read(prompt);
    }

    /// <inheritdoc/>
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        ReadLine.HistoryEnabled = true;
        ReadLine.AutoCompletionHandler =
            new RebootAutoCompleteHandler(dispatcher, Executive.DirCache);
    }
}