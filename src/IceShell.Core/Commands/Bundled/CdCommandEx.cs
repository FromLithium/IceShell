// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Commands.Complex;
using global::IceShell.Core.FileSystem;
using NexusKrop.IceShell.Core.FileSystem;

/// <summary>
/// Displays the name of or changes the current directory.
/// </summary>
[CommandInfo("cd")]
[CommandAlias("chdir")]
[GreedyString]
public class CdCommandEx : IShellCommand
{
    /// <summary>
    /// Gets or sets the directory to change the current directory to.
    /// </summary>
    /// <value>
    /// The directory to change the current directory to. If <see langword="null"/>, displays the name of the
    /// current directory.
    /// </value>
    [Value("destination", position: 0)]
    public string? Destination { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(Destination))
        {
            // Print current directory if no current directory is provided
            Console.WriteLine(Environment.CurrentDirectory);
            return CommandResult.Ok();
        }

        var target = PathSearcher.ExpandVariables(Destination) ?? "";

        if (!Directory.Exists(target))
        {
            return CommandResult.WithBadDirectory(target);
        }

        Executive.ChangeDirectory(target);

        return CommandResult.Ok();
    }
}
