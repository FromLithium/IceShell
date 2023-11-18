// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Argument;

/// <summary>
/// Supports enumeration argument conversion.
/// </summary>
public class EnumArgumentConverter : IArgumentConverter
{
    /// <inheritdoc />
    public bool TryConvert(string from, Type targetType, out object? result)
    {
        result = null;
        
        if (!targetType.IsEnum)
        {
            return false;
        }

        if (!Enum.TryParse(targetType, from, true, out result))
        {
            return false;
        }

        return true;
    }
}
