// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;
using System;

/// <summary>
/// Represents a command registration entry.
/// </summary>
/// <param name="Type">The type containing the implementation of the command.</param>
/// <param name="Id">The unique identifier of the command.</param>
/// <param name="OSPlatform">The platform that the command explicitly supports. If empty, the command is considered to work on all platforms.</param>
/// <param name="Definition">The command definition.</param>
/// <param name="Description">The description to show in help messages.</param>
public sealed record CommandEntry(Type Type, string Id, string[] OSPlatform, CommandDefinition Definition, string? Description = null);
