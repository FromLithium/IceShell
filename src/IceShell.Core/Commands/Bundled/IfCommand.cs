// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Attributes;
using NexusKrop.IceShell.Core.Commands.Complex;
using System;

/// <summary>
/// Checks for a condition.
/// </summary>
[CommandInfo("if")]
public class IfCommand : IShellCommand
{
    /// <summary>
    /// An enumeration of possible condition check types.
    /// </summary>
    public enum IfConditionType
    {
        /// <summary>
        /// Matches name or number of an error code with the last error code.
        /// </summary>
        ErrorCode,
        /// <summary>
        /// Matches the last exit code.
        /// </summary>
        ExitCode,
        /// <summary>
        /// Matches an environment variable.
        /// </summary>
        Variable,
        /// <summary>
        /// Determines whether a file exists.
        /// </summary>
        FileExists,
        /// <summary>
        /// Determines whether a directory exists.
        /// </summary>
        DirectoryExists
    }

    /// <summary>
    /// Gets or sets the comparison value type.
    /// </summary>
    [Value("matchType", true, 0)]
    public IfConditionType MatchType { get; set; }

    /// <summary>
    /// Gets or sets the name of the value (or an unnamed value) to compare.
    /// </summary>
    [Value("matchName", true, 1)]
    public string? MatchName { get; set; }

    /// <summary>
    /// Gets or sets the value to compare.
    /// </summary>
    [Value("matchValue", false, 2)]
    public string? MatchValue { get; set; }

    /// <summary>
    /// Gets or sets that whether to ignore case (in the case of a string comparison).
    /// </summary>
    [Option('I', false)]
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets whether to invert the test result.
    /// </summary>
    [Option('U', false)]
    public bool Invert { get; set; }

    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var matchOk = false;

        if (MatchName is null)
        {
            return CommandResult.WithMissingValue(1);
        }

        switch (MatchType)
        {
            case IfConditionType.Variable:
                var envValue = Environment.GetEnvironmentVariable(MatchName);

                // If MatchValue is not provided, check if the variable exists.
                if (string.IsNullOrEmpty(MatchValue))
                {
                    matchOk = envValue != null;
                    break;
                }

                // If MatchValue is provided but the environment variable does not exit, returns false.
                if (envValue is null)
                {
                    break;
                }

                // Performs the comparison.
                matchOk = envValue.Equals(MatchValue, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                break;
            case IfConditionType.ExitCode:
                if (!int.TryParse(MatchName, out var checkCode))
                {
                    return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.Get("command_if_match_value_not_number"));
                }

                matchOk = checkCode == shell.LastExitCode;
                break;
            case IfConditionType.ErrorCode:
                if (int.TryParse(MatchName, out var checkError))
                {
                    matchOk = ((int)shell.LastError) == checkError;
                    break;
                }

                if (!Enum.TryParse<IfConditionType>(MatchName, true, out var checkVal))
                {
                    return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.Get("command_if_match_value_not_vaild_error_code"));
                }

                matchOk = checkVal.Equals(shell.LastError);
                break;
            case IfConditionType.FileExists:
                if (!string.IsNullOrWhiteSpace(MatchValue))
                {
                    return CommandResult.WithError(CommandErrorCode.BadArgument,
                        LangMessage.Get("command.if.match_existence_but_value"));
                }

                matchOk = File.Exists(MatchName);
                break;
            case IfConditionType.DirectoryExists:
                if (!string.IsNullOrWhiteSpace(MatchValue))
                {
                    return CommandResult.WithError(CommandErrorCode.BadArgument,
                        LangMessage.Get("command.if.match_existence_but_value"));
                }

                matchOk = Directory.Exists(MatchName);
                break;
        }

        if (Invert)
        {
            matchOk = !matchOk;
        }

        return matchOk ? CommandResult.Ok() : CommandResult.WithCode(1);
    }
}
