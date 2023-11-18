// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

/// <summary>
/// Describes a value parameter.
/// </summary>
public struct CommandValueInfo
{
    /// <summary>
    /// Gets or sets the name used to refer to this value parameter in documentation.
    /// </summary>
    public string ReferenceName { get; set; }

    /// <summary>
    /// Gets or sets the position of this value parameter in the command format.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this value parameter is required.
    /// </summary>
    public bool Required { get; set; }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Position);
        hashCode.Add(Required);
        return hashCode.ToHashCode();
    }
}