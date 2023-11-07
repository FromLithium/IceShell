// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands;

using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Commands.Bundled;
using global::IceShell.Core.Commands.Complex;
using global::IceShell.Core.Exceptions;
using global::IceShell.Core.Api;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.Commands.Bundled;
using NexusKrop.IceShell.Core.Commands.Complex;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

/// <summary>
/// Provides services for the registration and lookup for the registered commands.
/// </summary>
public class CommandManager : ICommandManager
{
    private static readonly Type[] DefaultCommands = new Type[]
    {
        typeof(DirCommandEx),
        typeof(EchoCommandEx),
        typeof(ExitCommandEx),
        typeof(CdCommandEx),
        typeof(CopyCommandEx),
        typeof(ShellVersionCommand),
        typeof(StartCommandEx),
        typeof(ClearScreenCommandEx),
        typeof(MakeDirectoryCommand),
        typeof(DeleteFileCommandEx),
        typeof(MakeFileCommand),
        typeof(MoveCommandEx),
        typeof(TypeCommandEx),
        typeof(PromptCommandEx),
        typeof(HelpCommandEx),
        typeof(SetCommand),
        typeof(IfCommand)
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandManager"/> class.
    /// </summary>
    /// <param name="registerDefaults">If <see langword="true"/>, registers the bundled commands.</param>
    public CommandManager(bool registerDefaults = true)
    {
        CommandEntries = new ReadOnlyDictionary<string, CommandEntry>(_complexCommands);

        if (registerDefaults)
        {
            DefaultCommands.ForEach(Register);
        }
    }

    private readonly Dictionary<string, CommandEntry> _complexCommands = new();

    /// <summary>
    /// Gets the registered command entries.
    /// </summary>
    public IReadOnlyDictionary<string, CommandEntry> CommandEntries { get; }

    /// <inheritdoc />
    public int CommandCount => _complexCommands.Count;

    /// <inheritdoc />
    public IEnumerable<string> CommandAliases => _complexCommands.Keys;

    /// <inheritdoc />
    public CommandEntry? GetDefinition(string alias)
    {
        if (!_complexCommands.TryGetValue(alias.ToUpperInvariant(), out var x))
        {
            x = null;
        }

        if (x?.OSPlatform.IsEmpty() == false && !Array.Exists(x.OSPlatform, OperatingSystem.IsOSPlatform))
        {
            return null;
        }

        return x;
    }

    /// <inheritdoc />
    public bool HasDefinition(string alias)
    {
        if (!_complexCommands.TryGetValue(alias.ToUpperInvariant(), out var def))
        {
            return false;
        }

        if (def.OSPlatform.Any())
        {
            return Array.Exists(def.OSPlatform, OperatingSystem.IsOSPlatform);
        }
        else
        {
            return true;
        }
    }

    private static CommandDefinition GetDefine(Type type)
    {
        var definition = new CommandDefinition();
        var commandAttr = type.GetCustomAttribute<CommandInfoAttribute>()!;

        if (!string.IsNullOrWhiteSpace(commandAttr.CustomUsage))
        {
            definition.CustomUsage = commandAttr.CustomUsage;
        }

        // TODO generate description (See #2)
        definition.Description = "[Awaiting issue #2 completion]";

        if (type.GetCustomAttribute<GreedyStringAttribute>() != null)
        {
            definition.Greedy();
        }

        if (type.GetCustomAttribute<VariableValueAttribute>() != null)
        {
            definition.VarValues();
        }

        foreach (var property in type.GetProperties())
        {
            property.GetCustomAttributes().ForEach(x =>
            {
                var isValue = false;

                if (x is VariableValueBufferAttribute)
                {
                    definition.VariableValueBuffer = property;
                    return true;
                }

                // Register values first
                if (x is ValueAttribute valAttr)
                {
                    isValue = true;
                    definition.Value(new(valAttr.Name, property, valAttr.Required), valAttr.Position);
                }

                // Register options
                if (x is OptionAttribute optAttr)
                {
                    // If is already defined as value, throw
                    if (isValue)
                    {
                        throw new InvalidOperationException("A value cannot be option and value at the same time!");
                    }

                    if (!optAttr.HasValue && property.PropertyType != typeof(bool))
                    {
                        throw new InvalidOperationException($"Attempting to register property {property.Name} without value and property is not boolean");
                    }

                    definition.Option(optAttr.Character, optAttr.HasValue, property, optAttr.Required);
                }

                return true;
            });
        }

        return definition;
    }

    /// <inheritdoc />
    public void Register(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var platforms = new List<string>();

        // Step 1: Search command attributes
        var attributes = type.GetCustomAttributes(typeof(CommandInfoAttribute), false);

        if (attributes.Length != 1)
        {
            throw new ArgumentException(LangMessage.GetFormat("api_more_than_one_attribute", nameof(CommandInfoAttribute), type.FullName ?? type.Name), nameof(type));
        }

        // Step 2: Search platform attributes
        foreach (var attr in type.GetCustomAttributes(typeof(SupportedOSPlatformAttribute), false))
        {
            if (attr is SupportedOSPlatformAttribute os)
            {
                platforms.Add(os.PlatformName);
            }
        }

        // Step 3: Search alias attributes
        var aliasAttr = type.GetCustomAttributes(typeof(CommandAliasAttribute), false);

        // Step 4: Create definitions
        var definition = GetDefine(type);

        // Step 5: Verify definition
        if (definition.VariableValues && (definition.VariableValueBuffer == null
            || !definition.VariableValueBuffer.PropertyType.IsAssignableFrom(typeof(ReadOnlyCollection<string>))))
        {
            throw new InvalidOperationException(LangMessage.GetFormat("api_var_values_no_buffer", type.FullName ?? "<null>"));
        }

        // Now begin registering
        var intf = type.GetInterface(nameof(IShellCommand));

        if (intf != typeof(IShellCommand))
        {
            throw ExceptionHelper.CommandNoInterface(type, nameof(IShellCommand));
        }

        if (attributes[0] is not CommandInfoAttribute attribute)
        {
            throw new ArgumentException(LangMessage.GetFormat("api_command_invalid_attribute", type.FullName ?? "<null>"), nameof(type));
        }

        _complexCommands.Add(attribute.Name.ToUpperInvariant(), new(type, attribute.Name, platforms.ToArray(), definition, "Awaiting description change (#2)"));

        // Register all of its aliases
        foreach (var attr in aliasAttr)
        {
            if (attr is CommandAliasAttribute alias)
            {
                _complexCommands.Add(alias.Alias.ToUpperInvariant(), new(type, attribute.Name, platforms.ToArray(), definition, "TODO #2"));
            }
        }
    }

    /// <inheritdoc />
    public bool Any()
    {
        return _complexCommands.Count > 0;
    }
}
