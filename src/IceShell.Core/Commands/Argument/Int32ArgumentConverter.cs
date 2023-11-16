// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using System;

/// <summary>
/// Provides conversion for 32-bit integral types.
/// </summary>
public class Int32ArgumentConverter : IArgumentConverter
{
    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        if (!int.TryParse(from, out var retVal))
        {
            result = null;
            return false;
        }

        result = retVal;
        return true;
    }
}
