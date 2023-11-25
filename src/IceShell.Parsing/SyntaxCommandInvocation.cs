// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;
using System.Collections.Generic;

/// <summary>
/// Represents an invocation sentence that invokes an internal command.
/// </summary>
public record SyntaxCommandInvocation
{
    /// <summary>
    /// Initializes a new instance of <see cref="SyntaxCommandInvocation"/> class.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="options">The list of options.</param>
    /// <param name="values">The list of values.</param>
    public SyntaxCommandInvocation(string name, ICollection<SyntaxOption> options, IList<SyntaxWord> values)
    {
        Name = name;
        Options = options;
        Values = values;
    }

    /// <summary>
    /// Gets the name of the command that were specified by the user.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the list of options that were specified by the user.
    /// </summary>
    public ICollection<SyntaxOption> Options { get; }

    /// <summary>
    /// Gets the list of users that were specified by the user.
    /// </summary>
    public IList<SyntaxWord> Values { get; }
}
