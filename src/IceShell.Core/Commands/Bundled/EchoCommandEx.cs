// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;
using System;
using global::IceShell.Core.FileSystem;

/// <summary>
/// Displays messages.
/// </summary>
[CommandInfo(Id)]
[GreedyString]
public class EchoCommandEx : IShellCommand
{
    /// <summary>
    /// The command ID of the echo command.
    /// </summary>
    public const string Id = "echo";

    /// <summary>
    /// Gets or sets the message to display.
    /// </summary>
    /// <value>
    /// The message to display. If <see langword="null"/>, displays a blank new line.
    /// </value>
    [Value("message", position: 0, required: false)]
    public string? Message { get; set; }

    /// <summary>
    /// Whether to expand environment variables.
    /// </summary>
    [Option('E', false)]
    public bool ExpandVariables { get; set; }

    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (context.Retrieval == null)
        {
            var finalText = Message;

            if (ExpandVariables && Message != null)
            {
                finalText = PathSearcher.ExpandVariables(Message, true);
            }

            Console.WriteLine(finalText ?? Environment.NewLine);
        }
        else
        {
            Console.WriteLine(context.Retrieval.ReadToEnd());
        }

        return CommandResult.Ok();
    }
}
