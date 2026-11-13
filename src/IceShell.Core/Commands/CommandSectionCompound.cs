// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a collection of batches.
/// </summary>
public class CommandSectionCompound : IEnumerable<CommandSection>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandSectionCompound"/> class.
    /// </summary>
    /// <param name="lines">A list containing the batch line instances.</param>
    public CommandSectionCompound(IEnumerable<CommandSection> lines)
    {
        Lines = lines;
    }

    /// <summary>
    /// Gets a list containing the batch line instances.
    /// </summary>
    public IEnumerable<CommandSection> Lines { get; }

    /// <inheritdoc />
    public IEnumerator<CommandSection> GetEnumerator()
    {
        return Lines.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Lines).GetEnumerator();
    }

    /// <summary>
    /// Creates a <see cref="CommandSectionCompound"/> with only a single command.
    /// </summary>
    /// <param name="line">The command.</param>
    /// <returns>A new instance <see cref="CommandSectionCompound"/> with only a single command.</returns>
    public static CommandSectionCompound Single(CommandSection line)
    {
        return new CommandSectionCompound([line]);
    }

    /// <summary>
    /// Creates an empty <see cref="CommandSectionCompound"/>.
    /// </summary>
    /// <returns>An empty instance of <see cref="CommandSectionCompound"/>.</returns>
    public static CommandSectionCompound Empty()
    {
        return new CommandSectionCompound(Array.Empty<CommandSection>());
    }
}
