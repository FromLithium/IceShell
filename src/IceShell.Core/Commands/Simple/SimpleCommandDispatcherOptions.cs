// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Simple;

/// <summary>
/// Provides configuration options for the <see cref="SimpleCommandDispatcher"/>.
/// </summary>
public sealed class SimpleCommandDispatcherOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether a command name that does not denote a file path
    /// is always resolved to an external command, even when an internal command with the same
    /// name exists.
    /// </summary>
    public bool ResolveNonFilePathToExternalCommand { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a file that is not directly executable may be
    /// opened using the shell execute facility.
    /// </summary>
    public bool UseShellExecuteForNonExecutableFiles { get; set; }
}
