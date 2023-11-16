// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;
using System.Reflection;

/// <summary>
/// Supports string arguments.
/// </summary>
internal class StringArgumentConverter : IArgumentConverter
{
    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        result = from;
        return true;
    }
}
