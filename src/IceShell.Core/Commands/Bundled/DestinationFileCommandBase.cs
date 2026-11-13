// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;
using Microsoft.Extensions.FileSystemGlobbing;
using NexusKrop.IceShell.Core.FileSystem;
using System.Collections.Generic;
using IceShell.Core.FileSystem;
using IceShell.Core.Utilities;

/// <summary>
/// Provides a base class for file operation commands that involves multiple sources and a single destination.
/// </summary>
[Obsolete("Use DestinationFileCommandContext with new syntax instead.")]
public abstract class DestinationFileCommandBase
{
    /// <summary>
    /// Gets or sets the buffer to store values.
    /// </summary>
    public IReadOnlyList<string>? Buffer { get; set; }

    /// <summary>
    /// Gets or sets whether to execute the operation regardless of the existence of existing files at the destination.
    /// </summary>
    [Option('f', false)]
    public bool Force { get; set; }

    /// <summary>
    /// Performs the operation.
    /// </summary>
    /// <param name="source">The file to operate.</param>
    /// <param name="destination">The calculated destination of the specified file.</param>
    public abstract void DoOperation(string source, string destination);

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        string? destination = null;

        if (Buffer == null || Buffer.Count < 2)
        {
            return CommandResult.WithMissingValue(0);
        }

        var matcher = new Matcher();

        for (int i = 0; i < Buffer.Count; i++)
        {
            var str = PathSearcher.ExpandVariables(Buffer[i]);

            if (i == Buffer.Count - 1)
            {
                destination = Path.GetFullPath(str);
                continue;
            }

            matcher.AddInclude(str);
        }

        var destIsDir = Directory.Exists(destination) || destination!.EndsWith(Path.DirectorySeparatorChar);

        if (destIsDir)
        {
            CommandChecks.DirectoryExists(destination);
        }

        var realFiles = matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
        var toCopy = new Dictionary<string, string>();

        foreach (var file in realFiles)
        {
            if (!File.Exists(file))
            {
                continue;
            }

            if (destIsDir)
            {
                toCopy.Add(file, Path.Combine(destination, Path.GetFileName(file)));
            }
            else
            {
                toCopy.Add(file, destination);
            }
        }

        if (!destIsDir && toCopy.Count > 2)
        {
            return CommandResult.WithError(CommandErrorCode.SingleDestinationMultiSource);
        }

        toCopy.ForEach(x => DoOperation(x.Key, x.Value));

        return CommandResult.Ok();
    }
}
