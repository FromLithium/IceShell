// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using IceShell.Core.Utilities;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using NexusKrop.IceShell.Core.CLI;

/// <summary>
/// Deletes one or more files.
/// </summary>
public static class DeleteFileCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("del", Routine)
            .SwitchOption('P')
            .VariableValues()
            .NoParseVariables()
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandDocument executor, CommandContext context)
    {
        var targets = context.Arguments.OriginalWords;
        var toPrompt = context.GetSwitch('P');
        
        if (!targets.Any())
        {
            throw ExceptionHelper.RequiresValue(0);
        }

        var searchDir = targets.Count == 1 
            ? Path.GetDirectoryName(targets[0].Content)!
            : Directory.GetCurrentDirectory();

        if (string.IsNullOrWhiteSpace(searchDir))
        {
            searchDir = Directory.GetCurrentDirectory();
        }

        // Match via file glob
        var matcher = new Matcher();

        targets.ForEach(x => matcher.AddInclude(PathSearcher.ExpandVariables(x.Content)));

        var matchingResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(searchDir)));

        var realTargets = new List<string>(targets.Count);
        
        matchingResult.Files.ForEach(x => realTargets.Add(x.Path));

        if (realTargets.Count == 0)
        {
            return CommandResult.WithError(CommandErrorCode.BadFile);
        }

        if (realTargets.Count > 1)
        {
            foreach (var target in realTargets)
            {
                if (!File.Exists(target))
                {
                    return CommandResult.WithBadFile(target);
                }

                DeleteFileCommit(target, toPrompt);
            }
        }
        else
        {
            var target = realTargets[0];

            CommandChecks.FileExists(target);

            // Reserved for future use
            DeleteFileCommit(target, toPrompt);

        }

        return CommandResult.Ok();
    }

    private static void DeleteFileCommit(string file, bool prompt)
    {
        try
        {
            if (prompt && !ConsoleAsk.YesNo(LangMessage.GetFormat("del_prompt", file)))
            {
                return;
            }

            File.Delete(file);
        }
        catch (UnauthorizedAccessException)
        {
            throw new CommandFormatException(LangMessage.MsgUnauthorizedFile(file));
        }
    }
}
