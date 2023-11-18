// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

/// <summary>
/// Describes a value parameter.
/// </summary>
public struct CommandValueInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandValueInfo"/> structure.
    /// </summary>
    /// <param name="referenceName">The reference name of the value.</param>
    /// <param name="position">The position of the value.</param>
    /// <param name="required">If <see langword="true"/>, this value is required for a command to function.</param>
    public CommandValueInfo(string referenceName, int position, bool required = true)
    {
        ReferenceName = referenceName;
        Position = position;
        Required = required;
    }
    
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