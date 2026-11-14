namespace IceShell.Core.CLI.Editor;

using IceShell.Core.Api.Commands;

/// <summary>
/// Defines the standardised abstraction for line editors.
/// </summary>
public interface ILineEditor
{
    /// <summary>
    /// Initializes the line editor for use.
    /// </summary>
    /// <param name="dispatcher">The command dispatcher.</param>
    void Initialize(IRegistryCommandDispatcher dispatcher);

    /// <summary>
    /// Reads the input, beginning from the current cursor to the end of the line.
    /// </summary>
    /// <param name="dispatcher">The command dispatcher.</param>
    /// <param name="prompt">The prompt.</param>
    /// <returns>The user input read.</returns>
    string? DoReadLine(IRegistryCommandDispatcher dispatcher,
        string? prompt);
}