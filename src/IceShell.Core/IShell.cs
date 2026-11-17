// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core;

using IceShell.Core.Api;
using IceShell.Core.Commands.Simple;

/// <summary>
/// Represents a command-line shell interpreter.
/// </summary>
public interface IShell
{
    /// <summary>
    /// Gets or sets the prompt of this instance.
    /// </summary>
    string Prompt { get; set; }

    /// <summary>
    /// Gets the command dispatcher for this instance.
    /// </summary>
    ISimpleCommandDispatcher Dispatcher { get; }

    /// <summary>
    /// Gets the module manager for this instance.
    /// </summary>
    IModuleManager ModuleManager { get; }

    /// <summary>
    /// Gets the error code returned by the last command.
    /// </summary>
    CommandErrorCode LastError { get; }
    
    /// <summary>
    /// Gets the exit code returned by the last command.
    /// </summary>
    int LastExitCode { get; }

    /// <summary>
    /// Instructs the active shell process to stop receiving new commands after the current one
    /// is complete.
    /// </summary>
    void Quit();

    /// <summary>
    /// Parses a command and execute it.
    /// </summary>
    /// <param name="line">The line to parse and execute.</param>
    /// <param name="document">The command documemnt.</param>
    /// <returns>The return code of the command.</returns>
    CommandResult Execute(string line, ICommandDocument? document = null);
}
