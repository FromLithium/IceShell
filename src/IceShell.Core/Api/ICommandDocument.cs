// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.Commands;

/// <summary>
/// Defines a command document.
/// </summary>
public interface ICommandDocument
{
    /// <summary>
    /// Gets whether this instance supports skipping through lines.
    /// </summary>
    bool IsJumpSupported { get; }

    /// <summary>
    /// Skips to the specified label.
    /// </summary>
    /// <param name="label">The label to skip to.</param>
    void Jump(string label);
    
    /// <summary>
    /// Gets the mutable dictionary of variables.
    /// </summary>
    IDictionary<string, string>? Variables { get; }
}