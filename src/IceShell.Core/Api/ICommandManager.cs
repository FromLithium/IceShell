// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.Commands;

/// <summary>
/// Represents a command manager that provides registration services.
/// </summary>
/// <seealso cref="NexusKrop.IceShell.Core.Commands.CommandManager"/>
public interface ICommandManager
{
    /// <summary>
    /// Gets the total count of the commands registered to this instance.
    /// </summary>
    public int CommandCount { get; }

    /// <summary>
    /// Gets an enumerable for the name of the commands.
    /// </summary>
    public IEnumerable<string> CommandAliases { get; }

    /// <summary>
    /// Determines whether at least one command are registered with this instance.
    /// </summary>
    /// <returns><see langword="true"/> if at least one command are registered with this instance; otherwise, <see langword="false"/>.</returns>
    bool Any();

    /// <summary>
    /// Searches for an registration entry from the specified alias that supports
    /// the current environment.
    /// </summary>
    /// <param name="alias">The alias to look up with.</param>
    /// <returns>If found, the command entry; otherwise, <see langword="null"/>.</returns>
    CommandEntry? GetDefinition(string alias);

    /// <summary>
    /// Determines whether the manager have an registration entry with the specified alias
    /// that supports the current environment.
    /// </summary>
    /// <param name="alias">The alias to look up with.</param>
    /// <returns><see langword="true" /> if found; otherwise, <see langword="false"/>.</returns>
    bool HasDefinition(string alias);

    /// <summary>
    /// Determines whether the specified alias is registered. This method does not check if
    /// the command that the alias points to actually exists.
    /// </summary>
    /// <param name="alias">The alias to check.</param>
    /// <returns><see langword="true" /> if found; otherwise, <see langword="false"/>.</returns>
    bool HasAlias(string alias);

    /// <summary>
    /// Determines whether a command registered with the specified ID exists.
    /// </summary>
    /// <param name="commandId">The case-sensitive ID to search for.</param>
    /// <returns><see langword="true" /> if found; otherwise, <see langword="false"/>.</returns>
    bool HasCommand(string commandId);

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="type">The command implementation type.</param>
    /// <exception cref="ArgumentException">The command implementation type or its set of attributes are invalid.</exception>
    /// <exception cref="InvalidOperationException">The command implementation type is invalid.</exception>
    /// <remarks>
    /// <para>
    /// You will need to add a correct set of attributes to the properties and the type itself in order for it to correctly register and function.
    /// For more information, consult the documentation or the IceShell source code.
    /// </para>
    /// </remarks>
    void Register(Type type);

    /// <summary>
    /// Registers an alias bound to the specified command. The command may or may not be registered.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="commandId">The identifier of the command to bound to.</param>
    /// <exception cref="ArgumentException">The command alias is already registered.</exception>
    void RegisterAlias(string alias, string commandId);
}
