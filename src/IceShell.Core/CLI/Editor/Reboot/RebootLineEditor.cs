namespace IceShell.Core.CLI.Editor.Reboot;

using IceShell.Core.Api.Commands;
using ReadLineReboot;


internal sealed class RebootLineEditor : ILineEditor
{
    public string? DoReadLine(IRegistryCommandDispatcher dispatcher, string? prompt)
    {
        return ReadLine.Read(prompt);
    }

    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        ReadLine.HistoryEnabled = true;
        ReadLine.AutoCompletionHandler =
            new RebootAutoCompleteHandler(dispatcher, Executive.DirCache);
    }
}