// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides utilities for parsing commands.
/// </summary>
public static class CommandParser
{
    /// <summary>
    /// The statement that specifies that all statements after it are values.
    /// </summary>
    public const string EndOfOptionsStatement = "--";

    /// <summary>
    /// Parses a compound of commands.
    /// </summary>
    /// <param name="statements">The statements.</param>
    /// <param name="isCommandName">A predicate to check if the name provided is a valid command.</param>
    /// <param name="handleHomePath">A function to convert home path.</param>
    /// <returns>The parsed compound.</returns>
    public static SyntaxCompound ParseCompound(IReadOnlyList<SyntaxWord> statements,
        Predicate<string> isCommandName,
        Func<string, string>? handleHomePath)
    {
        var thisCompound = new List<SyntaxSegment>();
        var currentSegment = new List<SyntaxWord>();

        foreach (var statement in statements)
        {
            if (statement.Content == "&")
            {
                EndSegment(SyntaxNextAction.Continue);
            }
            else if (statement.Content == "&&")
            {
                EndSegment(SyntaxNextAction.IfSuccessOnly);
            }
            else if (statement.Content == ">")
            {
                EndSegment(SyntaxNextAction.Redirect);
            }
            else
            {
                currentSegment.Add(statement);
                continue;
            }

            currentSegment.Clear();
        }

        EndSegment(SyntaxNextAction.None);

        void EndSegment(SyntaxNextAction nextAction)
        {
            var fileName = currentSegment![0].Content;

            if (handleHomePath != null && fileName.StartsWith('~'))
            {
                fileName = handleHomePath(fileName);
            }
            
            if (!fileName.StartsWith('.') && isCommandName(fileName))
            {
                thisCompound.Add(new SyntaxSegment(ParseSingleCommand(currentSegment),
                    nextAction));
            }
            else
            {
                SyntaxWord[] array;

                if (currentSegment.Count == 1)
                {
                    array = Array.Empty<SyntaxWord>();
                }
                else
                {
                    array = new SyntaxWord[currentSegment.Count - 1];
                    currentSegment.CopyTo(1, array, 0, currentSegment.Count - 1);
                }

                thisCompound.Add(new SyntaxSegment(new ExternalCommandRef(fileName, array),
                    nextAction));
            }
        }

        return new(thisCompound.AsReadOnly());
    }

    /// <summary>
    /// Parses a single command.
    /// </summary>
    /// <param name="statements">The list of statements that constitutes of a command.</param>
    /// <returns>The parsed command.</returns>
    /// <exception cref="FormatException">The command syntax is invalid.</exception>
    public static SyntaxCommandInvocation ParseSingleCommand(IReadOnlyList<SyntaxWord> statements)
    {
        var options = new HashSet<SyntaxOption>();
        var values = new List<SyntaxWord>();
        var name = "<pending>";
        var ignoreNextValue = false;
        var endOfOptions = false;

        if (!statements.Any())
        {
            name = string.Empty;
            return new(name, options, values);
        }

        for (int i = 0; i < statements.Count; i++)
        {
            var statement = statements[i];
            var content = statement.Content;

            if (i == 0)
            {
                name = content;
                continue;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                continue;
            }

            if (!endOfOptions
                && content == EndOfOptionsStatement
                && !statement.WasQuoted)
            {
                endOfOptions = true;
                continue;
            }

            if (endOfOptions || !IsOption(statement))
            {
                // Is value!
                if (!ignoreNextValue)
                {
                    values.Add(statement);
                }

                ignoreNextValue = false;
                continue;
            }

            SyntaxWord? next = null;

            if (statements.Count > i + 1)
            {
                next = statements[i + 1];
            }

            var option = ParseOption(statement, next, out var extended);

            if (extended)
            {
                ignoreNextValue = true;
            }

            options.Add(option);
        }

        return new(name, options, values);
    }

    /// <summary>
    /// Parses a single option.
    /// </summary>
    /// <param name="word">The first statement.</param>
    /// <param name="next">The statement directly after the first statement.</param>
    /// <param name="extended">Whether the next value is used to parse the option. The output value of this parameter should be used to tell the parser to skip the next value, if needed.</param>
    /// <returns>The parsed option.</returns>
    /// <exception cref="FormatException">The option syntax is invalid.</exception>
    public static SyntaxOption ParseOption(SyntaxWord word, SyntaxWord? next, out bool extended)
    {
        var content = word.Content;
        string? value;

        var delimiter = content[1];

        if (content.Length == 2)
        {
            extended = false;
            value = null;
        }
        else if (content.Length == 3 && content.EndsWith(':'))
        {
            if (next == null)
            {
                throw new FormatException(string.Format("Next statement is required for option '{0}'", delimiter));
            }

            if (!next.WasQuoted)
            {
                throw new FormatException(string.Format("Invalid option '{0}'. Is parsing failure?", delimiter));
            }

            extended = true;
            value = next.Content;
        }
        else
        {
            extended = false;
            value = content[3..];
        }

        return new(delimiter, value);
    }

    internal static bool IsOption(SyntaxWord word)
    {
        var content = word.Content;

        if (word.WasQuoted)
        {
            return false;
        }

        if (content.Length < 2)
        {
            return false;
        }

        if (!content.StartsWith('/'))
        {
            return false;
        }

        if (content.Length > 2 && content[2] != ':')
        {
            return false;
        }

        return true;
    }
}
