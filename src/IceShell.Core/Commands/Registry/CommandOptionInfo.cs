// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

/// <summary>
/// Represents an option with value.
/// </summary>
/// <param name="Identifier">The identifier.</param>
/// <param name="IsSwitch">If <see langword="true"/>, this option does not have a value.</param>
public record struct CommandOptionInfo(char Identifier, bool IsSwitch)
{
    /// <summary>
    /// Gets or sets the identifier of the option.
    /// </summary>
    public char Identifier { get; set; } = Identifier;

    /// <summary>
    /// Gets or sets a value indicating whether this option is registered as a switch.
    /// </summary>
    public bool IsSwitch { get; set; } = IsSwitch;

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Identifier);
        hashCode.Add(IsSwitch);
        return hashCode.ToHashCode();
    }
}