// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using System.Reflection;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Argument;
using IceShell.Core.Exceptions;
using NexusKrop.IceCube.Collections;

internal class AnnotatedCommandProvider
{
    private readonly IRegistryCommandDispatcher _dispatcher;
    private readonly Dictionary<CommandOptionInfo, PropertyInfo> _options = new();
    private readonly Dictionary<CommandValueInfo, PropertyInfo> _values = new();
    private readonly ArgumentConvertService _converter = new();

    internal AnnotatedCommandProvider(IRegistryCommandDispatcher dispatcher, Type type, PropertyInfo? variableBuffer = null)
    {
        _dispatcher = dispatcher;
        this.Type = type;
        VariableBuffer = variableBuffer;
    }

    internal Type Type { get; }
    
    internal PropertyInfo? VariableBuffer { get; set; }
    
    internal void SetProperty(CommandOptionInfo argument, string value, IShellCommand instance)
    {
        if (!_options.TryGetValue(argument, out var property))
        {
            throw new ArgumentException("The specified argument is not registered with this provider.", nameof(argument));
        }
        
        CheckForConversion(property);
        if (!_converter.TryConvert(value, property.PropertyType, out var result))
        {
            throw new CommandInterruptException(CommandResult.WithError(CommandErrorCode.BadArgument));
        }
        
        property.SetValue(instance, result);
    }

    internal void SetSwitchProperty(CommandOptionInfo argument, IShellCommand instance)
    {
        if (!_options.TryGetValue(argument, out var property))
        {
            throw new ArgumentException("The specified option switch is not registered with this provider.", nameof(argument));
        }
        
        property.SetValue(instance, true);
    }

    internal void SetProperty(CommandValueInfo argument, string value, IShellCommand instance)
    {
        if (!_values.TryGetValue(argument, out var property))
        {
            throw new ArgumentException("The specified argument is not registered with this provider.", nameof(argument));
        }
        
        CheckForConversion(property);
        if (!_converter.TryConvert(value, property.PropertyType, out var result))
        {
            throw new CommandInterruptException(CommandResult.WithError(CommandErrorCode.BadArgument));
        }
        
        property.SetValue(instance, result);
    }
    
    internal void AddOption(CommandOptionInfo option, PropertyInfo property)
    {
        if (option.IsSwitch)
        {
            AddSwitch(option, property);
        }
        else
        {
            AddValueOption(option, property);
        }
    }

    internal void AddValue(CommandValueInfo info, PropertyInfo property, bool ignoreConverterCheck = false)
    {
        if (property.PropertyType != typeof(string) && !ignoreConverterCheck)
        {
            CheckForConversion(property);
        }
        
        _values.Add(info, property);
    }
    
    internal void AddValueOption(CommandOptionInfo option, PropertyInfo property, bool ignoreConverterCheck = false)
    {
        if (property.PropertyType != typeof(string) && !ignoreConverterCheck)
        {
            CheckForConversion(property);
        }
        
        _options.Add(option, property);
    }

    private void CheckForConversion(PropertyInfo property)
    {
        if (_converter == null)
        {
            throw new ArgumentException("Argument type conversion is not supported.", nameof(property));
        }

        if (!_converter.CanConvertTo(property.PropertyType))
        {
            throw new ArgumentException("Conversion of argument to the property type is not supported.", nameof(property));
        }
    }

    private void AddSwitch(CommandOptionInfo option, PropertyInfo property)
    {
        if (property.PropertyType != typeof(bool))
        {
            throw new ArgumentException("Unable to register the specified property as a switch because the property is not of boolean.", nameof(property));
        }
        
        _options.Add(option, property);
    }
}