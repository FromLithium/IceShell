// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using IceShell.Core.Api;
using IceShell.Core.Exceptions;
using NexusKrop.IceShell.Core.CLI;
using System;
using System.Collections.Generic;
using System.Reflection;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;

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

    internal bool TryConvert(string from, PropertyInfo target, IShellCommand instance)
    {
        var targetType = target.PropertyType;

        if (targetType.IsEnum)
        {
            targetType = typeof(Enum);
        }

        if (!_converters.TryGetValue(targetType, out var converter))
        {
            return false;
        }

        if (!TryConvert(from, target.PropertyType, out var result))
        {
            ConsoleOutput.PrintShellError(LangMessage.MsgFromErrorCode(CommandErrorCode.BadArgument));
            return false;
        }

        try
        {
            target.SetValue(instance, result);
        }
        catch (CommandFormatException ex)
        {
            ConsoleOutput.PrintShellError(ex.Message);
            return false;
        }

        return true;
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
        return _converters.ContainsKey(type);
    }
}
