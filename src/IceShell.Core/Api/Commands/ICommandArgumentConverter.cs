// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

using System.Reflection;
using IceShell.Core.Commands.Argument;

/// <summary>
/// Represents a conversion service that converts arguments to their corresponding types.
/// </summary>
public interface ICommandArgumentConverter : IArgumentConverter
{
    /// <summary>
    /// Registers the specified converter and associates it to the specified type.
    /// </summary>
    /// <param name="type">The type to register converter to.</param>
    /// <param name="converter">The converter to register.</param>
    /// <returns>This instance, for chaining.</returns>
    ICommandArgumentConverter RegisterConverter(Type type, IArgumentConverter converter);
    
    /// <summary>
    /// Determines whether a string argument value can be converted to the specified
    /// <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <returns><see langword="true"/> if this instance can convert to <paramref name="type"/>; otherwise, <see langword="false"/>.</returns>
    bool CanConvertTo(Type type);
}