// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using System;
using System.IO;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Defines a command that displays or modifies an environment variable.
/// </summary>
public static class SetCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("set", Routine)
            .ValueParameter("name")
                .Position(0)
                .Required()
                .NoExpandVariables()
                .Build()
            .ValueParameter("value", 1)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var variableName = context.GetValue("name");
        var variableValue = context.GetValue("value");

        if (string.IsNullOrWhiteSpace(variableName))
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument);
        }

        if (string.IsNullOrWhiteSpace(variableName))
        {
            return CommandResult.WithMissingValue(0);
        }

        if (variableValue is null)
        {
            var envVar = Environment.GetEnvironmentVariable(variableName);

            var pipeStream = envVar == null ? TextReader.Null : new StringReader(envVar);

            return CommandResult.Ok(pipeStream);
        }

        Environment.SetEnvironmentVariable(variableName, variableValue);

        return CommandResult.Ok();
    }
}
