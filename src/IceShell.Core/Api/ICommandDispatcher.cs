// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.Api.Commands;
using IceShell.Core.Commands;
using IceShell.Core.Exceptions;

/// <summary>
/// Represents a command dispatcher.
/// </summary>
[Obsolete("Use ICommandRegistryDispatcher.")]
public interface ICommandDispatcher
{
    /// <summary>
    /// Gets the command manager for this instance.
    /// </summary>
    public ICommandManager CommandManager { get; }
    
    /// <summary>
    /// Gets the argument conversion service for this instance.
    /// </summary>
    public ICommandArgumentConverter? ArgumentConverter { get; }

    /// <summary>
    /// Executes the specified compound of command sections. If any of the sections in the compound returned
    /// non-zero value, the execution is interrupted and this method returns with the return value of such section.
    /// </summary>
    /// <param name="compound">The compound to execute.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <returns>The execution result.</returns>
    /// <exception cref="CommandInterruptException">The command was interrupted.</exception>
    /// <exception cref="CommandFormatException">There was a legacy format error.</exception>
    CommandResult Execute(CommandSectionCompound compound, ICommandExecutor executor);

    /// <summary>
    /// Executes the specified command section.
    /// </summary>
    /// <param name="section">The section to execute.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="context">The execution context of this execution. Each context must be specific to execution of each section.</param>
    /// <returns>The execution result.</returns>
    /// <exception cref="CommandInterruptException">The command was interrupted.</exception>
    /// <exception cref="CommandFormatException">There was a legacy format error.</exception>
    CommandResult Execute(CommandSection section, ICommandExecutor executor, ExecutionContext? context = null);

    /// <summary>
    /// Executes the specified unit.
    /// </summary>
    /// <param name="command">The unit to execute.</param>
    /// <param name="executor">The executor to act on behalf of.</param>
    /// <param name="context">The execution context of this execution.</param>
    /// <returns>The execution result.</returns>
    /// <exception cref="CommandInterruptException">The command was interrupted.</exception>
    /// <exception cref="CommandFormatException">There was a legacy format error.</exception>
    CommandResult Execute(CommandUnit command, ICommandExecutor executor, ExecutionContext context);

    /// <summary>
    /// Parse the specified line of command to a <see cref="CommandSectionCompound"/>.
    /// </summary>
    /// <param name="line">The line to parse from.</param>
    /// <returns>A new instance of <see cref="CommandSectionCompound"/>.</returns>
    /// <exception cref="CommandFormatException">A malformed or invalid command was found. For more information, consult the message of the exception.</exception>
    CommandSectionCompound ParseLine(string line);
}
