// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;

/// <summary>
/// Defines a command dispatcher that is based on the registry command system. It is meant to
/// replace both <see cref="ICommandManager"/> and <see cref="ICommandDispatcher"/> interfaces.
/// </summary>
public interface IRegistryCommandDispatcher
{
    /// <summary>
    /// Gets the amount of aliases that is registered with the dispatcher.
    /// </summary>
    int AliasCount { get; }

    /// <summary>
    /// Enumerates all aliases.
    /// </summary>
    /// <returns>An enumerable of aliases.</returns>
    IEnumerable<string> EnumerateAliases();

    /// <summary>
    /// Enumerates all aliases associated with the specified command.
    /// </summary>
    /// <param name="commandId">The ID of the command.</param>
    /// <returns>An enumerable of all aliases associated with the specified command.</returns>
    IEnumerable<string> EnumerateAliases(string commandId);

    /// <summary>
    /// Determines whether there are any command associated with this instance.
    /// </summary>
    /// <returns><see langword="true"/> if found; otherwise, <see langword="false"/>.</returns>
    bool AnyCommand();

    /// <summary>
    /// Determines whether there is a command with the specified identifier associated with this instance.
    /// </summary>
    /// <param name="id">The identifier to look up for.</param>
    /// <returns><see langword="true"/> if found; otherwise, <see langword="false"/>.</returns>
    bool AnyCommand(string id);
    
    /// <summary>
    /// Registers the specified command to its delegate.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <param name="routine">The routine to assign it to.</param>
    void Register(CommandRegistryEntry command, CommandDelegate routine);

    /// <summary>
    /// Searches for an entry associated with the specified alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="result">The search result.</param>
    /// <returns><see langword="true"/> if found; otherwise, <see langword="false"/>.</returns>
    bool SearchInfoByAlias(string alias, out CommandRegistryEntry? result);

    /// <summary>
    /// Determines whether a command with the specified alias is registered with this instance.
    /// </summary>
    /// <param name="alias">The ID to search.</param>
    /// <returns><see langword="true"/> if the specified ID is found; otherwise, <see langword="false"/>.</returns>
    bool HasAlias(string alias);

    /// <summary>
    /// Sets the specified alias associated with the specified identifier.
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="id"></param>
    void PutAlias(string alias, string id);
    
    /// <summary>
    /// Parses the specified command line for execution at a later time.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>The parse result.</returns>
    CommandSectionCompound Parse(string line)
    {
        return Parse((ReadOnlySpan<char>)line);
    }

    /// <summary>
    /// Parses the specified command line for execution at a later time.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>The parse result.</returns>
    CommandSectionCompound Parse(ReadOnlySpan<char> line);

    /// <summary>
    /// Parses the specified segment to its internal command equivalent.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>The parsed section.</returns>
    CommandSection ParseSection(SyntaxSegment segment);
    
    /// <summary>
    /// Parses and then executes the specified command line.
    /// </summary>
    /// <param name="shell">The shell to execute on behalf of.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="line">The line to parse.</param>
    /// <returns>The result of the command execution.</returns>
    CommandResult Execute(IShell shell, ICommandDocument document, string line);
    
    /// <summary>
    /// Executes an already parsed command line.
    /// </summary>
    /// <param name="shell">The shell to execute on behalf of.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="context">The execution context.</param>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The result of the command execution.</returns>
    CommandResult Execute(IShell shell, ICommandDocument document, ExecutionContext context, RegistryCommandParseResult parseResult);

    /// <summary>
    /// Executes an already parsed internal or external command.
    /// </summary>
    /// <param name="shell">The shell to execute the section as.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="context">The context.</param>
    /// <param name="section">The section to execute.</param>
    /// <returns>The result of the command execution.</returns>
    CommandResult Execute(IShell shell, ICommandDocument document, ExecutionContext context, CommandSection section);
    
    /// <summary>
    /// Executes a compound of already parsed commands.
    /// </summary>
    /// <param name="shell">The shell to execute the compound as.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="compound">The compound to execute.</param>
    /// <returns>The result of the command execution.</returns>
    CommandResult Execute(IShell shell, ICommandDocument document, CommandSectionCompound compound);
}