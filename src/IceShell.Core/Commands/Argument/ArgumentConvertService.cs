// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using System;
using System.Collections.Generic;
using IceShell.Core.Api.Commands;

internal class ArgumentConvertService : ICommandArgumentConverter
{
    private readonly Dictionary<Type, IArgumentConverter> _converters = new();

    internal ArgumentConvertService RegisterConverter(Type type, IArgumentConverter converter)
    {
        _converters.Add(type, converter);
        return this;
    }

    internal void UnregisterConverter(Type type)
    {
        _converters.Remove(type);
    }
    
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        var searchType = targetType;
        
        if (searchType.IsEnum)
        {
            searchType = typeof(Enum);
        }
        
        if (!_converters.TryGetValue(searchType, out var converter))
        {
            result = default;
            return false;
        }

        if (!converter.TryConvert(from, targetType, out var retVal))
        {
            result = default;
            return false;
        }

        result = retVal;
        return true;
    }

    public bool CanConvertTo(Type type)
    {
        // This means that if the type is an enum, it is treated as the base Enum type
        // which matches the Enum converter since it must treat all generic enumeration converters,
        // unless another converter is registered specifically for that enumeration.
        if (!_converters.ContainsKey(type) && type.IsEnum)
        {
            type = typeof(Enum);
        }
        
        return _converters.ContainsKey(type);
    }
}
