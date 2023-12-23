// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that prints out the contents of a file.
/// </summary>
public static class TypeCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("type", Routine)
            .ValueParameter("file", 0, true)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var argFile = context.GetValue("file");
        
        if (string.IsNullOrWhiteSpace(argFile))
        {
            return CommandResult.WithMissingValue(0);
        }

        var realFile = PathSearcher.ExpandVariables(argFile!);
        CommandChecks.FileExists(realFile);

        return CommandResult.Ok(new StreamReader(realFile));
    }
}
