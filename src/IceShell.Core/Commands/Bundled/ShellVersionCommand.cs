// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using NexusKrop.IceShell.Core.Commands.Complex;
using System;

/// <summary>
/// Defines a command that shows the version of the operating system and the shell program.
/// </summary>
[CommandInfo("ver")]
public class ShellVersionCommand : IShellCommand
{
    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        Console.WriteLine(LangMessage.Get("ver_line_0"), Shell.AppVersion);
        Console.WriteLine(LangMessage.Get("ver_line_1"), Environment.OSVersion.VersionString);
        Console.WriteLine();
        Console.WriteLine(LangMessage.Get("ver_line_2"));

        return CommandResult.Ok();
    }
}
