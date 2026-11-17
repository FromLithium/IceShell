// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Simple;

using IceShell.Core.Api;
using IceShell.Parsing;

/// <summary>
/// Defines a command dispatcher for the simple command system.
/// </summary>
public interface ISimpleCommandDispatcher
{
    /// <summary>
    /// Gets the amount of internal commands currently registered with this dispatcher.
    /// </summary>
    int CommandCount { get; }

    /// <summary>
    /// Determines whether an internal command with the specified name is registered.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <returns><see langword="true"/> if the command exists; otherwise, <see langword="false"/>.</returns>
    bool HasCommand(string name);

    /// <summary>
    /// Searches for an internal command with the specified name.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="command">The command action, if found.</param>
    /// <returns><see langword="true"/> if the command exists; otherwise, <see langword="false"/>.</returns>
    bool TryGetCommand(string name, out InternalCommand? command);

    /// <summary>
    /// Enumerates the names of all registered internal commands.
    /// </summary>
    /// <returns>An enumerable of command names.</returns>
    IEnumerable<string> EnumerateCommandNames();

    /// <summary>
    /// Executes the command represented by the specified word list.
    /// </summary>
    /// <param name="shell">The shell that is executing the command.</param>
    /// <param name="document">The command document.</param>
    /// <param name="words">The words that make up the command line.</param>
    /// <returns>The exit code of the command.</returns>
    int Execute(IShell shell, ICommandDocument document, IReadOnlyList<SyntaxWord> words);
}
