// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;

/// <summary>
/// Defines a command that creates an empty file.
/// </summary>
public static class MakeFileCommand
{
    /// <summary>
    ///     Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("mkfile", Routine)
            .NoParseVariables()
            .ValueParameter("file", 0, true)
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandDocument executor, CommandContext context)
    {
        var actual = PathSearcher.ExpandVariables(context.GetValue("file"));

        CommandChecks.FileNotExists(actual);
        CommandChecks.DirectoryNotExists(actual);

        try
        {
            // Create an absolutely empty file
            File.WriteAllBytes(actual, Array.Empty<byte>());
        }
        catch (UnauthorizedAccessException)
        {
            throw ExceptionHelper.UnauthorizedWrite();
        }

        return CommandResult.Ok();
    }
}