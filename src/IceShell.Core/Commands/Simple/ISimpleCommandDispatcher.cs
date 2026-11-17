// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Simple;

using IceShell.Core.Api;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;

/// <summary>
/// Defines a command dispatcher for the simple command system.
/// </summary>
public interface ISimpleCommandDispatcher
{
    /// <summary>
    /// Gets the amount of aliases currently registered with this dispatcher.
    /// </summary>
    int AliasCount { get; }

    /// <summary>
    /// Gets the amount of internal commands currently registered with this dispatcher.
    /// </summary>
    int CommandCount { get; }

    /// <summary>
    /// Registers an internal command with the specified name.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="command">The command action.</param>
    /// <exception cref="ArgumentException">A command with the same name already exists.</exception>
    void Register(string name, InternalCommand command);

    /// <summary>
    /// Registers an internal command with the specified name and optional registry information.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="command">The command action.</param>
    /// <param name="info">The registry entry that describes the command, if any.</param>
    /// <exception cref="ArgumentException">A command with the same name already exists.</exception>
    void Register(string name, InternalCommand command, CommandRegistryEntry? info);

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
    /// Searches for the registry information of an internal command with the specified name.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="info">The registry information, if found.</param>
    /// <returns><see langword="true"/> if the information exists; otherwise, <see langword="false"/>.</returns>
    bool TryGetCommandInfo(string name, out CommandRegistryEntry? info);

    /// <summary>
    /// Enumerates the names of all registered internal commands.
    /// </summary>
    /// <returns>An enumerable of command names.</returns>
    IEnumerable<string> EnumerateCommandNames();

    /// <summary>
    /// Sets an alias to the specified command line. Any existing alias with the same name is
    /// replaced.
    /// </summary>
    /// <param name="alias">The name of the alias.</param>
    /// <param name="commandLine">The command line the alias expands to.</param>
    void SetAlias(string alias, string commandLine);

    /// <summary>
    /// Removes the alias with the specified name.
    /// </summary>
    /// <param name="alias">The name of the alias.</param>
    /// <returns><see langword="true"/> if the alias was removed; otherwise, <see langword="false"/>.</returns>
    bool RemoveAlias(string alias);

    /// <summary>
    /// Determines whether an alias with the specified name is registered.
    /// </summary>
    /// <param name="alias">The name of the alias.</param>
    /// <returns><see langword="true"/> if the alias exists; otherwise, <see langword="false"/>.</returns>
    bool HasAlias(string alias);

    /// <summary>
    /// Searches for the command line associated with the specified alias.
    /// </summary>
    /// <param name="alias">The name of the alias.</param>
    /// <param name="commandLine">The command line, if found.</param>
    /// <returns><see langword="true"/> if the alias exists; otherwise, <see langword="false"/>.</returns>
    bool TryGetAlias(string alias, out string? commandLine);

    /// <summary>
    /// Enumerates the names of all registered aliases.
    /// </summary>
    /// <returns>An enumerable of alias names.</returns>
    IEnumerable<string> EnumerateAliases();

    /// <summary>
    /// Enumerates the names of all aliases that expand to the specified command name.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns>An enumerable of alias names.</returns>
    IEnumerable<string> EnumerateAliases(string commandName);

    /// <summary>
    /// Enumerates all aliases and their associated command lines.
    /// </summary>
    /// <returns>An enumerable of alias pairs.</returns>
    IEnumerable<KeyValuePair<string, string>> EnumerateAliasValues();

    /// <summary>
    /// Executes the command represented by the specified word list.
    /// </summary>
    /// <param name="shell">The shell that is executing the command.</param>
    /// <param name="document">The command document.</param>
    /// <param name="words">The words that make up the command line.</param>
    /// <returns>The exit code of the command.</returns>
    int Execute(IShell shell, ICommandDocument document, IReadOnlyList<SyntaxWord> words);
}
