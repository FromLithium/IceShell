// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using System;
using System.IO;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
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

        var actualName = variableName[1..^1];

        // Context variables.
        if (variableName.StartsWith('$'))
        {
            // Check for context variable availability.
            if (document.Variables == null)
            {
                return CommandResult.WithError(CommandErrorCode.OperationNotSupported,
                    LangMessage.Get("command.set.error.no_context_variable_support"));
            }
            
            // If ends with the context variable symbol, means to get or set the value.
            if (variableName.EndsWith('$'))
            {
                DoContext(variableValue, actualName, document, out var pipeStream);
                return CommandResult.Ok(pipeStream);
            }
            
            // If ends with equal symbol, means to delete the value.
            // ReSharper disable once InvertIf
            if (variableName.EndsWith('='))
            {
                document.Variables.Remove(actualName);
                return CommandResult.Ok();
            }

            return ErrorInvalidVariableName();
        }
        
        // Environment variables.
        // ReSharper disable once InvertIf
        if (variableName.StartsWith('%'))
        {
            if (variableName.EndsWith('%'))
            {
                DoEnvironment(variableValue, actualName, out var pipeStream);
                return CommandResult.Ok(pipeStream);
            }
            
            // ReSharper disable once InvertIf
            if (variableName.EndsWith('='))
            {
                Environment.SetEnvironmentVariable(actualName, null);
                return CommandResult.Ok();
            }
        }
        
        return ErrorInvalidVariableName();
    }

    private static CommandResult ErrorInvalidVariableName()
    {
        return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.Get("command.set.error.invalid_variable_name"));
    }

    private static void DoContext(string? variableValue, string variableName, ICommandDocument document, out TextReader? pipeStream)
    {
        if (document.Variables is null)
        {
            throw new InvalidOperationException("Variables not supported.");
        }
        
        if (variableValue is null)
        {
            pipeStream = document.Variables!.TryGetValue(variableName, out var displayValue)
                ? new StringReader(displayValue)
                : TextReader.Null;

            return;
        }

        document.Variables![variableName] = variableValue;
        pipeStream = null;
    }

    private static void DoEnvironment(string? variableValue, string variableName, out TextReader? pipeStream)
    {
        if (variableValue is null)
        {
            var envVar = Environment.GetEnvironmentVariable(variableName);

            pipeStream = envVar == null ? TextReader.Null : new StringReader(envVar);

            return;
        }

        Environment.SetEnvironmentVariable(variableName, variableValue);
        pipeStream = null;
    }
}
