// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;

/// <summary>
/// Represents an option specified by the user.
/// </summary>
public readonly struct SyntaxOption : IEquatable<SyntaxOption>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxOption"/> structure.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="value">The value.</param>
    public SyntaxOption(char identifier, string? value)
    {
        Identifier = identifier;
        Value = value;
    }

    /// <summary>
    /// Gets the identifier of this option.
    /// </summary>
    public char Identifier { get; }

    /// <summary>
    /// Gets the value of this option.
    /// </summary>
    /// <value>
    /// The value specified; <see langword="null"/> if there were no value.
    /// </value>
    public string? Value { get; }

    /// <summary>
    /// Determines whether the specified instance represents the same option argument as this instance.
    /// </summary>
    /// <param name="other">The instance to compare with.</param>
    /// <returns>
    /// <see langword="true"/> if the specified instance represents the same option argument as this instance;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(SyntaxOption other)
    {
        return Identifier.Equals(other.Identifier) &&
               string.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is SyntaxOption option
               && Equals(option);
    }

    /// <summary>
    /// Gets the hash code of this instance.
    /// </summary>
    /// <returns>The hash code of this instance.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Identifier);
        hash.Add(Value);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Determines whether the value to the <paramref name="left"/> is equivalent to the value to the <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The value at the left side of the operator.</param>
    /// <param name="right">The value at the right side of the operator.</param>
    /// <returns>
    /// <see langword="true"/> if the specified instances represents the same option argument;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(SyntaxOption left, SyntaxOption right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the value to the <paramref name="left"/> is not equivalent to the value to the <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The value at the left side of the operator.</param>
    /// <param name="right">The value at the right side of the operator.</param>
    /// <returns>
    /// <see langword="true"/> if the specified instances does not represent the same option argument;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(SyntaxOption left, SyntaxOption right)
    {
        return !(left == right);
    }
}
