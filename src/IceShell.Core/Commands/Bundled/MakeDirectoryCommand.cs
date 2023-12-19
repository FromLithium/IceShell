// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using System;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Complex;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that creates either a directory or a tree of directories.
/// </summary>
[CommandInfo("mkdir")]
[CommandAlias("md")]
public static class MakeDirectoryCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("mkdir", Routine)
            .Alias("md")
            .NoParseVariables()
            .ValueParameter("directory", 0, true)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var dir = PathSearcher.ExpandVariables(context.GetValue("directory"));
        
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
