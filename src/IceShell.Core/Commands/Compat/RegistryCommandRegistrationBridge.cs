// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Compat;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Commands.Simple;
using IceShell.Parsing;

/// <summary>
/// Provides a compatibility layer that allows commands written for the registry command system
/// to be registered to a <see cref="ISimpleCommandDispatcher"/>.
/// </summary>
internal sealed class RegistryCommandRegistrationBridge : IRegistryCommandDispatcher
{
    private readonly ISimpleCommandDispatcher _dispatcher;
    private readonly Dictionary<string, CommandRegistryEntry> _entries =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _aliases = new(StringComparer.OrdinalIgnoreCase);

    internal RegistryCommandRegistrationBridge(ISimpleCommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public int AliasCount => _aliases.Count;

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases()
    {
        return _aliases.Keys;
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases(string commandId)
    {
        return _aliases.Where(x => x.Value.Equals(commandId, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Key);
    }

    /// <inheritdoc />
    public bool AnyCommand()
    {
        return _entries.Count > 0;
    }

    /// <inheritdoc />
    public bool AnyCommand(string id)
    {
        return _entries.ContainsKey(id);
    }

    /// <inheritdoc />
    public void Register(CommandRegistryEntry command, CommandDelegate routine)
    {
        _dispatcher.Register(command.Identifier, CommandCompat.FromRegistry(command, routine), command);
        _entries[command.Identifier] = command;

        // The registry dispatcher always exposes the identifier itself as an alias.
        _aliases[command.Identifier] = command.Identifier;
    }

    /// <inheritdoc />
    public bool SearchInfoByAlias(string alias, out CommandRegistryEntry? result)
    {
        if (!_aliases.TryGetValue(alias, out var id))
        {
            result = null;
            return false;
        }

        return _entries.TryGetValue(id, out result);
    }

    /// <inheritdoc />
    public bool HasAlias(string alias)
    {
        return _aliases.ContainsKey(alias);
    }

    /// <inheritdoc />
    public void PutAlias(string alias, string id)
    {
        if (!_entries.ContainsKey(id))
        {
            throw new ArgumentException("The specified ID does not exist.", nameof(id));
        }

        _aliases[alias] = id;

        if (!alias.Equals(id, StringComparison.OrdinalIgnoreCase))
        {
            _dispatcher.SetAlias(alias, id);
        }
    }

    /// <inheritdoc />
    public CommandSectionCompound Parse(ReadOnlySpan<char> line, ICommandDocument? document)
    {
        throw new NotSupportedException("The registry command parser is not available in the simple command system.");
    }

    /// <inheritdoc />
    public CommandSection ParseSection(SyntaxSegment segment, ICommandDocument? document)
    {
        throw new NotSupportedException("The registry command parser is not available in the simple command system.");
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, string line)
    {
        throw new NotSupportedException("The registry command executor is not available in the simple command system.");
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell,
        ICommandDocument document,
        ExecutionContext context,
        RegistryCommandParseResult parseResult)
    {
        throw new NotSupportedException("The registry command executor is not available in the simple command system.");
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell,
        ICommandDocument document,
        ExecutionContext context,
        CommandSection section)
    {
        throw new NotSupportedException("The registry command executor is not available in the simple command system.");
    }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandDocument document, CommandSectionCompound compound)
    {
        throw new NotSupportedException("The registry command executor is not available in the simple command system.");
    }
}
