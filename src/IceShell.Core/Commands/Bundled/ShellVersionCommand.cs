// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;
using NexusKrop.IceShell.Core;
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
        Console.WriteLine(LangMessage.Get("shell.attribution.version"), Shell.AppVersion);
        Console.WriteLine(LangMessage.Get("shell.attribution.copyright_1"));

        if (ShowLicenceInfo)
        {
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.redistribute"));
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.warranty"));
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.link"));
        }

        if (ShowSystemInfo)
        {
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.system.os"), Environment.OSVersion.VersionString);
            Console.WriteLine(LangMessage.Get("command.ver.system.runtime"), Environment.Version.ToString());
        }

        return CommandResult.Ok();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show licence information.
    /// </summary>
    [Option('C', false)]
    public bool ShowLicenceInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the system information is shown.
    /// </summary>
    [Option('S', false)]
    public bool ShowSystemInfo { get; set; }
}
