// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Exceptions;
using NexusKrop.IceCube.Exceptions;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;
using System;
using global::IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that creates an empty file.
/// </summary>
[CommandInfo("mkfile")]
public class MakeFileCommand : IShellCommand
{
    /// <summary>
    /// Gets or sets the name of the file to create.
    /// </summary>
    [Value("value", position: 0)]
    public string? Name { get; set; }

    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var actual = PathSearcher.ExpandVariables(Name);

        Checks.ArgNotNull(actual);
        CommandChecks.FileNotExists(actual!);
        CommandChecks.DirectoryNotExists(actual!);

        try
        {
            // Create an absolutely empty file
            File.WriteAllBytes(actual, Array.Empty<byte>());
        }
        catch (UnauthorizedAccessException)
        {
            throw ExceptionHelper.UnauthorizedWrite();
        }

        return CommandResult.Ok();
    }
}
