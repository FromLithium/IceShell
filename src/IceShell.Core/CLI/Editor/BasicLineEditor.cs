namespace IceShell.Core.CLI.Editor;

using IceShell.Core.Api.Commands;

/// <summary>
/// Provides a basic implementation of line reading and editing based on standard library function
/// <see cref="Console.ReadLine"/>.
/// </summary>
public sealed class BasicLineEditor : ILineEditor
{
    /// <inheritdoc/>
    public string? DoReadLine(IRegistryCommandDispatcher dispatcher, string? prompt)
    {
        if (!string.IsNullOrWhiteSpace(prompt))
        {
            Console.Write(prompt);
        }

        return Console.ReadLine();
    }

    /// <inheritdoc/>
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        // No need to setup.
    }

}
