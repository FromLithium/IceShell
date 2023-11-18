// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Exceptions;
using global::IceShell.Core.FileSystem;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that prints out the contents of a file.
/// </summary>
[CommandInfo("type")]
public class TypeCommandEx : IShellCommand
{
    /// <summary>
    /// The file to display.
    /// </summary>
    [Value("file", position: 0)]
    public string? ArgFile { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(ArgFile))
        {
            return CommandResult.WithMissingValue(0);
        }

        var realFile = PathSearcher.ExpandVariables(ArgFile!);
        CommandChecks.FileExists(realFile);

        ExecuteStreamed(realFile, out var pipeStream);

        return CommandResult.Ok(pipeStream);
    }

    private static void ExecuteStreamed(string file, out TextReader pipeStream)
    {
        pipeStream = new StreamReader(file);
    }
}
