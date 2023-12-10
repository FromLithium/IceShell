// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using System;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines a command that shows the version of the operating system and the shell program.
/// </summary>
public static class ShellVersionCommand
{
    private const char ShowCopyrightOption = 'C';
    private const char ShowSystemInfoOption = 'S';
    
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("ver", Routine)
            .SwitchOption('C')
            .SwitchOption('S')
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        Console.WriteLine(LangMessage.Get("shell.attribution.version"), Shell.AppVersion);
        Console.WriteLine(LangMessage.Get("shell.attribution.copyright_1"));

        if (context.GetSwitch(ShowCopyrightOption))
        {
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.redistribute"));
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.warranty"));
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.licence.link"));
        }

        // Justification: This is a switch option and it cannot be nested forward.
        // Invert here will break readability.
        //
        // ReSharper disable once InvertIf
        if (context.GetSwitch(ShowSystemInfoOption))
        {
            Console.WriteLine();
            Console.WriteLine(LangMessage.Get("command.ver.system.os"), Environment.OSVersion.VersionString);
            Console.WriteLine(LangMessage.Get("command.ver.system.runtime"), Environment.Version.ToString());
        }

        return CommandResult.Ok();
    }
}
