// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Attributes;
using IceShell.Core.Commands.Complex;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;

/// <summary>
/// Displays the name of or changes the current directory.
/// </summary>
[CommandInfo("cd")]
[CommandAlias("chdir")]
[GreedyString]
public static class CdCommand
{
    private const string ValueDestination = "destination";
    
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("cd", Routine)
            .Alias("chdir")
            .ValueParameter(ValueDestination, 0)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var destination = context.GetValue(ValueDestination);
        
        if (string.IsNullOrWhiteSpace(destination))
        {
            // Print current directory if no current directory is provided
            Console.WriteLine(Environment.CurrentDirectory);
            return CommandResult.Ok();
        }

        var target = PathSearcher.ExpandVariables(destination, document: document) ?? "";

        if (!Directory.Exists(target))
        {
            return CommandResult.WithBadDirectory(target);
        }

        Executive.ChangeDirectory(target);

        return CommandResult.Ok();
    }
}
