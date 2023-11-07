// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;
using System;

/// <summary>
/// Specifies information regarding commands. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CommandInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfoAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the command. This will became the unique case-sensitive ID as well as
    /// the first alias of the command.</param>
    public CommandInfoAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets or sets the name of the command.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the custom usage displayed in help information.
    /// </summary>
    public string? CustomUsage { get; set; }
}
