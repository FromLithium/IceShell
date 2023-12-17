// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using System.ComponentModel;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;
using JetBrains.Annotations;

/// <summary>
/// Defines a command that starts the specified file or executable program.
/// </summary>
public class StartCommandEx
{
    /// <summary>
    /// The unique identifier of this command.
    /// </summary>
    [PublicAPI]
    public const string CommandId = "start";

    private const string ValueParameterTarget = "target";
    
    /// <summary>
    /// Gets or sets the executable or the file to start.
    /// </summary>
    [Value("target", position: 0)]
    public string? Target { get; set; }

    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder(CommandId, Routine)
            .ValueParameter(ValueParameterTarget, 0, true)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var target = context.GetValue(ValueParameterTarget);
        
        if (target == null)
        {
            return CommandResult.WithMissingValue(0);
        }

        var realTarget = PathSearcher.ExpandVariables(target);

        CommandChecks.FileExists(realTarget);

        try
        {
            NexusKrop.IceCube.Util.Shell.ShellExecute(realTarget);
        }
        catch (Win32Exception x) when (x.NativeErrorCode == 1155)
        {
            return CommandResult.WithError(CommandErrorCode.OperatingSystemError, LangMessage.Get("start_bad_assoc"));
        }

        return CommandResult.Ok();
    }
}
