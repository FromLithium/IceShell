// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Compat;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;

/// <summary>
/// Provides a compatibility layer that translates commands written for the registry command
/// system into internal commands of the simple command system.
/// </summary>
internal sealed class RegistryCommandRegistrationBridge : IRegistryCommandDispatcher
{
    private readonly Dictionary<string, InternalCommand> _commands =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, CommandRegistryEntry> _entries =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _aliases = new(StringComparer.OrdinalIgnoreCase);

    internal IReadOnlyDictionary<string, InternalCommand> Commands => _commands;

    internal bool TryGetEntry(string name, out CommandRegistryEntry? entry)
    {
        return _entries.TryGetValue(name, out entry);
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
        _commands.Add(command.Identifier, CommandCompat.FromRegistry(command, routine));
        _entries.Add(command.Identifier, command);
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

        // This is a stopgap solution to not introduce behaviour mismatch.
        _commands[alias] = (shell, docs, args) => {
            return _commands[id](shell, docs, args);
        };
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
