// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

/// <summary>
/// Defines an interface that converts string value to other types.
/// </summary>
public interface IArgumentConverter
{
    /// <summary>
    /// Converts from the specified string value to the specified result.
    /// </summary>
    /// <param name="from">The string to convert from.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="result">The conversion result.</param>
    /// <returns><see langword="true"/> if successful; otherwise, <see langword="false"/>.</returns>
    public bool TryConvert(string from, Type targetType, out object? result);
}
