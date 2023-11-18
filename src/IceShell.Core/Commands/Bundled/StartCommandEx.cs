// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Exceptions;
using NexusKrop.IceCube.Exceptions;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;
using System.ComponentModel;
using global::IceShell.Core.FileSystem;

/// <summary>
/// Starts the specified file or executable program.
/// </summary>
[CommandInfo("start")]
public class StartCommandEx : IShellCommand
{
    /// <summary>
    /// Gets or sets the executable or the file to start.
    /// </summary>
    [Value("target", position: 0)]
    public string? Target { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (Target == null)
        {
            return CommandResult.WithMissingValue(0);
        }

        var realTarget = PathSearcher.ExpandVariables(Target);

        CommandChecks.FileExists(realTarget);

        try
        {
            IceCube.Util.Shell.ShellExecute(realTarget);
        }
        catch (Win32Exception x) when (x.NativeErrorCode == 1155)
        {
            return CommandResult.WithError(CommandErrorCode.OperatingSystemError, LangMessage.Get("start_bad_assoc"));
        }

        return CommandResult.Ok();
    }
}
