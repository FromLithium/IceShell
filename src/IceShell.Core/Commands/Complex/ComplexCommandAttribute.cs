// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Complex;
using System;

/// <summary>
/// Specifies that the class attributed is a complex command.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[Obsolete("This attribute will no longer be recognised by the command manager. Use CommandInfoAttribute instead.", true)]
public sealed class ComplexCommandAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexCommandAttribute"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public ComplexCommandAttribute(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Gets or sets the name of the command.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the command.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the custom usage string of the command.
    /// </summary>
    /// <value>
    /// The custom usage string. If <see langword="null"/>, a usage string will be generated based on the options and values defined.
    /// </value>
    public string? CustomUsage { get; set; }
}