// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Complex;

using System.Reflection;

/// <summary>
/// Defines a complex value argument.
/// </summary>
/// <remarks>
/// A complex value argument is an argument specified based on a particular order instead of a name. In
/// IceShell, the order of value arguments is the order specified via the attributes.
/// </remarks>
/// <param name="Name">The name for reference.</param>
/// <param name="Required">Whether or not the argument is required.</param>
/// <param name="Property">The property to store the value.</param>
public record ComplexValueDefinition(string Name, PropertyInfo Property, bool Required = false);