// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using IceShell.Core.Exceptions;
using System;
using System.Reflection;

/// <summary>
/// Provides conversion for signed 8-bit integral types.
/// </summary>
public class SByteArgumentConverter : IArgumentConverter
{
    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        if (!sbyte.TryParse(from, out var retVal))
        {
            result = null;
            return false;
        }

        result = retVal;
        return true;
    }
}
