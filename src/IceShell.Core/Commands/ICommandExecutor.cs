// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

/// <summary>
/// Represents a service that can act as the executor and context provider of commands.
/// </summary>
[Obsolete("Use ICommandDocument instead.")]
public interface ICommandExecutor
{
    /// <summary>
    /// Gets whether this instance supports skipping through lines.
    /// </summary>
    bool SupportsJump { get; }

    /// <summary>
    /// Skips to the specified label.
    /// </summary>
    /// <param name="label">The label to skip to.</param>
    void Jump(string label);
}
