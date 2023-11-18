// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using IceShell.Core.Exceptions;
using System.Reflection;

/// <summary>
/// Provides conversion service for 64-bit integral types.
/// </summary>
public class Int64ArgumentConverter : IArgumentConverter
{
    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        if (!long.TryParse(from, out var retVal))
        {
            result = null;
            return false;
        }

        result = retVal;
        return true;
    }
}
