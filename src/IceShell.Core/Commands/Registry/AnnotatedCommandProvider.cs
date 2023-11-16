// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using System.Reflection;
using IceShell.Core.Api;
using IceShell.Core.Exceptions;
using NexusKrop.IceCube.Collections;

internal class AnnotatedCommandProvider
{
    private readonly ICommandDispatcher _dispatcher;
    private readonly Dictionary<CommandOptionInfo, PropertyInfo> _options = new();
    private readonly Dictionary<CommandValueInfo, PropertyInfo> _values = new();

    internal AnnotatedCommandProvider(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    internal void SetProperty(CommandValueInfo argument, string value, IShellCommand instance)
    {
        if (!_values.TryGetValue(argument, out var property))
        {
            throw new ArgumentException("The specified argument is not registered with this provider.", nameof(argument));
        }
        
        CheckForConversion(property);
        if (!_dispatcher.ArgumentConverter!.TryConvert(value, property.PropertyType, out var result))
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

    private void AddValue(CommandValueInfo info, PropertyInfo property, bool ignoreConverterCheck = false)
    {
        if (property.PropertyType != typeof(string) && !ignoreConverterCheck)
        {
            CheckForConversion(property);
        }
        
        _values.Add(info, property);
    }
    
    private void AddValueOption(CommandOptionInfo option, PropertyInfo property, bool ignoreConverterCheck = false)
    {
        if (property.PropertyType != typeof(string) && !ignoreConverterCheck)
        {
            CheckForConversion(property);
        }
        
        _options.Add(option, property);
    }

    private void CheckForConversion(PropertyInfo property)
    {
        if (_dispatcher.ArgumentConverter == null)
        {
            throw new ArgumentException("Argument type conversion is not supported.", nameof(property));
        }

        if (!_dispatcher.ArgumentConverter.CanConvertTo(property.PropertyType))
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