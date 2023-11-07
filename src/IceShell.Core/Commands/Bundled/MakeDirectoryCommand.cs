// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Commands.Complex;
using global::IceShell.Core.Exceptions;
using NexusKrop.IceCube.Exceptions;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;
using System;

/// <summary>
/// Creates a directory, or a tree of directories.
/// </summary>
[CommandInfo("mkdir")]
[CommandAlias("md")]
public class MakeDirectoryCommand : IShellCommand
{
    /// <summary>
    /// Gets or sets the name of the directory or that path syntax that represents the directory tree to create.
    /// </summary>
    [Value("directory", true, 0)]
    public string? DirectoryName { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var dir = PathSearcher.ExpandVariables(DirectoryName);

        Checks.ArgNotNull(DirectoryName);
        CommandChecks.NothingExists(dir);

        try
        {
            Directory.CreateDirectory(dir);
        }
        catch (UnauthorizedAccessException)
        {
            return CommandResult.WithError(CommandErrorCode.WriteUnauthorized);
        }

        return CommandResult.Ok();
    }
}
