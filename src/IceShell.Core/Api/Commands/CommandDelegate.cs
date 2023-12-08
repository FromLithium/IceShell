// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

using IceShell.Core.Commands;

/// <summary>
/// The command execution delegate.
/// </summary>
public delegate CommandResult CommandDelegate(IShell shell, ICommandDocument executor, CommandContext context);