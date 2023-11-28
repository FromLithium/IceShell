// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;
using IceShell.Parsing.Util;

public static class NewIfCommand
{
    private const string IfNot = "NOT";
    private const string IfElse = "ELSE";
    private const string IfErrorLevelSyntax = "ERRORLEVEL";
    
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("if", Routine)
            .VariableValues()
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandExecutor executor, CommandContext context)
    {
        if (!context.Arguments.OriginalWords.Any())
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument);
        }

        // If the NOT keyword is specified.
        var not = false;
        
        // Starts the actual command parsing at which index.
        var startAtIndex = 0;
        
        // The variable for convenience.
        var words = context.Arguments.OriginalWords;

        // The condition result.
        var condition = false;
        
        if (UnquotedIs(words[0], IfNot))
        {
            not = true;
            startAtIndex = 1;
        }

        if (words.Count < startAtIndex)
        {
            // Too few arguments.
            return CommandResult.WithError(CommandErrorCode.BadArgument);
        }

        var startWord = words[startAtIndex];

        // Environment variable.
        if (startWord.Content.Contains("=="))
        {
            var result = RunConditionVariable(startWord);
            if (result.ErrorCode != CommandErrorCode.None)
            {
                // Parsing failure.
                return CommandResult.WithError(result.ErrorCode);
            }

            condition = result.Result;
        }
        
        // Last exit code (error level).
        else if (UnquotedIs(words[startAtIndex], IfErrorLevelSyntax))
        {
            if (words.Count <= startAtIndex + 1)
            {
                // Too less arguments.
                return CommandResult.WithError(CommandErrorCode.BadArgument);
            }
            
            var result = RunConditionErrorLevel(shell, words[startAtIndex + 1]);
            if (result.ErrorCode != CommandErrorCode.None)
            {
                // Parsing failure.
                return CommandResult.WithError(result.ErrorCode);
            }

            condition = result.Result;
            // Push the start index once more because we used 2 words here.
            startAtIndex++;
        }

        // Push the start index to the next then command.
        startAtIndex++;

        if (words.Count <= startAtIndex)
        {
            return condition ? CommandResult.WithCode(0) : CommandResult.WithCode(1);
        }

        var thenCommand = words[startAtIndex];

        SyntaxWord? elseCommand = null;
        
        if (words.Count > startAtIndex + 1)
        {
            if (!UnquotedIs(words[startAtIndex + 1], IfElse))
            {
                // No other word than ELSE is accepted.
                return CommandResult.WithError(CommandErrorCode.BadArgument);
            }
            
            if (words.Count <= startAtIndex + 2)
            {
                // If ELSE is specified then the ELSE command must also be specified.
                return CommandResult.WithError(CommandErrorCode.BadArgument);
            }
            
            elseCommand = words[startAtIndex + 2];
        }
        
        if (thenCommand.QuoteType != WordQuoteType.Parentheses
        || (elseCommand != null && elseCommand.QuoteType != WordQuoteType.Parentheses))
        {
            // The THEN and ELSE commands must be surrounded by parentheses.
            return CommandResult.WithError(CommandErrorCode.BadArgument);
        }

        // If the condition matches.
        if (condition)
        {
            return shell.Execute(thenCommand.Content, executor);
        }

        // If the condition mismatches...
        // If ELSE command is available and is specified, execute the ELSE command.
        // Otherwise, return 1.
        return elseCommand != null ? shell.Execute(elseCommand.Content, executor) : CommandResult.WithCode(1);
    }

    private static CommandRoutineResult<bool> RunConditionVariable(SyntaxWord startWord)
    {
        var split = startWord.Content.Split("==");

        if (split.Length != 2)
        {
            return CommandErrorCode.BadArgument;
        }

        var variable = split[0];
        var value = split[1];

        var existingValue = Environment.GetEnvironmentVariable(variable);

        return string.Equals(existingValue, value);
    }

    private static CommandRoutineResult<bool> RunConditionErrorLevel(IShell shell, SyntaxWord word)
    {
        if (!int.TryParse(word.Content, out var value))
        {
            return CommandErrorCode.BadArgument;
        }

        return shell.LastExitCode == value;
    }

    private static bool QuoteAnyStartsWith(SyntaxWord word, string expected)
    {
        return word.Content.StartsWith(expected, StringComparison.CurrentCultureIgnoreCase);
    }
    
    private static bool UnquotedIs(SyntaxWord word, string expected)
    {
        return word.QuoteType == WordQuoteType.None &&
               word.Content.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}