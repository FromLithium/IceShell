// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using System.Diagnostics;
using System.Reflection;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Attributes;
using IceShell.Core.Commands.Utilities;
using NexusKrop.IceCube.Util.Enumerables;

/// <summary>
/// Provides services for registering old legacy annotated commands.
/// </summary>
public class AnnotatedCommandRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotatedCommandRegistry"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    public AnnotatedCommandRegistry(IRegistryCommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    private readonly Dictionary<CommandRegistryEntry, AnnotatedCommandProvider> _providers = new();
    private readonly IRegistryCommandDispatcher _dispatcher;

    /// <summary>
    /// Registers the specified command.
    /// </summary>
    /// <param name="type">The type to register.</param>
    public void Register(Type type)
    {
        var entry = ResolveAnnotatedCommand(type, out var provider);
        _providers.Add(entry, provider);
        
        _dispatcher.Register(entry, (shell, document, context) => 
            ExecuteAnnotatedCommand(entry, context, shell, new CommandDocumentAnnotationWrapper(document)));
    }
    
    internal CommandRegistryEntry ResolveAnnotatedCommand(Type type, out AnnotatedCommandProvider provider)
    {
        var commandAttr = type.GetCustomAttribute<CommandInfoAttribute>()!;
        var entry = new CommandRegistryEntry(commandAttr.Name);
        var outProvider = new AnnotatedCommandProvider(_dispatcher, type);
        
        if (!string.IsNullOrWhiteSpace(commandAttr.CustomUsage))
        {
            entry.CustomUsage = commandAttr.CustomUsage;
        }

        if (type.GetCustomAttribute<GreedyStringAttribute>() != null)
        {
            entry.GreedyString = true;
        }

        if (type.GetCustomAttribute<VariableValueAttribute>() != null)
        {
            entry.VariableValues = true;
        }

        Debug.WriteLine($"Command registry: {entry.Identifier} -> {type.FullName}");
        
        foreach (var property in type.GetProperties())
        {
            property.GetCustomAttributes().ForEach(x =>
            {
                switch (x)
                {
                    case VariableValueBufferAttribute:
                        outProvider.VariableBuffer = property;
                        return true;
                    // Register values first
                    case ValueAttribute valAttr:
                    {
                        var valInfo = entry.AddValue(valAttr.Name, valAttr.Position, valAttr.Required);
                        outProvider.AddValue(valInfo.ReferenceName, property);
                        break;
                    }
                    case OptionAttribute { HasValue: false } when property.PropertyType != typeof(bool):
                        throw new InvalidOperationException($"Attempting to register property {property.Name} without value and property is not boolean");
                    case OptionAttribute optAttr:
                    {
                        Debug.WriteLine($"  [Option] {optAttr.Character} (hasValue: {optAttr.HasValue}) -> {property.Name}");                        
                        var optInfo = entry.AddOption(optAttr.Character, !optAttr.HasValue);

                        if (optAttr.HasValue)
                        {
                            outProvider.AddValueOption(optInfo.Identifier, property);
                        }
                        else
                        {
                            outProvider.AddSwitch(optInfo.Identifier, property);
                        }
                        break;
                    }
                }

                return true;
            });
        }

        provider = outProvider;
        return entry;
    }
    
    internal AnnotatedCommandProvider CreateProvider(CommandRegistryEntry entry, Type commandType, PropertyInfo? variableBuffer)
    {
        var result = new AnnotatedCommandProvider(_dispatcher, commandType, variableBuffer);
        _providers.Add(entry, result);
        return result;
    }

    private CommandResult ExecuteAnnotatedCommand(CommandRegistryEntry entry,
        CommandContext context,
        IShell shell,
        ICommandExecutor executor)
    {
        var args = context.Arguments;
        if (!_providers.TryGetValue(entry, out var provider))
        {
            throw new InvalidOperationException("The specified command is not a registered annotated command.");
        }

        var instance = (IShellCommand?)Activator.CreateInstance(provider.Type);

        if (instance == null)
        {
            throw new InvalidOperationException("Cannot create instance of the annotated command.");
        }
        
        foreach (var switchVal in args.Switches.Where(x => x.Value))
        {
            provider.SetSwitchOption(switchVal.Key, switchVal.Value, instance);
        }

        foreach (var optionVal in args.ValueOptions)
        {
            provider.SetValueOption(optionVal.Key, optionVal.Value, instance);
        }

        foreach (var valueVal in args.Values)
        {
            provider.SetProperty(valueVal.Key, valueVal.Value, instance);
        }

        return instance.Execute(shell, executor, CreateContext(context));
    }

    internal static ExecutionContext CreateContext(CommandContext from)
    {
        return new ExecutionContext(from.Retrieval, from.NextAction);
    }
}