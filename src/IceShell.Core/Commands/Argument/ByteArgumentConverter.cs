// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

using IceShell.Core.Exceptions;
using System;
using System.Reflection;

/// <summary>
/// Provides conversion for unsigned 8-bit integral types.
/// </summary>
public class ByteArgumentConverter : IArgumentConverter
{
    /// <inheritdoc/>
    public void Convert(string from, PropertyInfo property, object instance)
    {
        if (property.PropertyType != typeof(byte))
        {
            throw new InvalidOperationException("Property is not Byte.");
        }

        if (!byte.TryParse(from, out var result))
        {
            throw ExceptionHelper.CannotResolve(from);
        }

        property.SetValue(instance, result);
    }

    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        result = null;
        
        // It cannot be boxed from here?
        if (!byte.TryParse(from, out var byteResult))
        {
            return false;
        }

        result = byteResult;
        return true;
    }
}
