// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using IceShell.Core.Api;
using IceShell.Parsing;

/// <summary>
/// Represents an internal command of the simple command system.
/// </summary>
/// <param name="shell">The shell that is executing the command.</param>
/// <param name="document">The command document.</param>
/// <param name="args">The arguments specified after the command name.</param>
/// <returns>The exit code of the command.</returns>
public delegate int InternalCommand(IShell shell,
    ICommandDocument document,
    IReadOnlyList<SyntaxWord> args);
