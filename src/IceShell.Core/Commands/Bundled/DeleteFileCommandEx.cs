// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Exceptions;
using global::IceShell.Core.Api;
using global::IceShell.Core.FileSystem;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.CLI;
using NexusKrop.IceShell.Core.Commands.Complex;
using NexusKrop.IceShell.Core.FileSystem;

/// <summary>
/// Deletes one or more files.
/// </summary>
[CommandInfo("del")]
[VariableValue]
public class DeleteFileCommandEx : IShellCommand
{
    /// <summary>
    /// Gets or sets the list of the targets.
    /// </summary>
    [VariableValueBuffer]
    public IReadOnlyList<string>? Targets { get; set; }

    /// <summary>
    /// Gets or sets whether to require confirmation for each file.
    /// </summary>
    [Option('P', false)]
    public bool Prompt { get; set; }

    private void DeleteFileCommit(string file)
    {
        try
        {
            if (Prompt && !ConsoleAsk.YesNo(LangMessage.GetFormat("del_prompt", file)))
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

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (Targets?.Any() != true)
        {
            throw ExceptionHelper.RequiresValue(0);
        }

        string searchDir;

        if (Targets.Count == 1)
        {
            searchDir = Path.GetDirectoryName(Targets[0])!;
        }
        else
        {
            searchDir = Directory.GetCurrentDirectory();
        }

        if (string.IsNullOrWhiteSpace(searchDir))
        {
            searchDir = Directory.GetCurrentDirectory();
        }

        var targets = new List<string>(Targets.Count);

        // Match via file glob
        var matcher = new Matcher();

        Targets.ForEach(x => matcher.AddInclude(PathSearcher.ExpandVariables(x)));

        var matchingResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(searchDir ?? Environment.CurrentDirectory)));

        matchingResult.Files.ForEach(x => targets.Add(x.Path));

        if (!targets.Any())
        {
            return CommandResult.WithError(CommandErrorCode.BadFile);
        }

        if (targets.Count > 1)
        {
            foreach (var target in targets)
            {
                if (!File.Exists(target))
                {
                    return CommandResult.WithBadFile(target);
                }

                DeleteFileCommit(target);
            }
        }
        else
        {
            var target = targets[0];

            CommandChecks.FileExists(target);

            // Reserved for future use
            DeleteFileCommit(target);

        }

        return CommandResult.Ok();
    }
}
