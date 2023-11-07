// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Commands.Attributes;
using System;
using System.IO;

/// <summary>
/// Displays or modifies an environment variable.
/// </summary>
[CommandInfo("SET")]
[GreedyString]
public class SetCommand : IShellCommand
{
    /// <summary>
    /// The name of the variable.
    /// </summary>
    [Value("variable", position: 0)]
    public string? VariableName { get; set; }

    /// <summary>
    /// The value of the variable. If not specified, displays the current value.
    /// </summary>
    [Value("value", position: 1, required: false)]
    public string? VariableValue { get; set; }

    /// <inheritdoc />
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        TextReader? pipeStream = null;

        if (string.IsNullOrWhiteSpace(VariableName))
        {
            return CommandResult.WithMissingValue(0);
        }

        if (VariableValue is null)
        {
            var envVar = Environment.GetEnvironmentVariable(VariableName);

            pipeStream = envVar == null ? TextReader.Null : new StringReader(envVar);

            return CommandResult.Ok();
        }

        Environment.SetEnvironmentVariable(VariableName, VariableValue);

        return CommandResult.Ok();
    }
}
