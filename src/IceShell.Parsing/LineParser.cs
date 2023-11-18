// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Provides services for extracting statements from a line.
/// </summary>
public class LineParser
{
    /// <summary>
    /// The escape character (back-slash).
    /// </summary>
    public const char EscapeChar = '\\';

    /// <summary>
    /// The double quote character.
    /// </summary>
    public const char DoubleQuote = '"';

    /// <summary>
    /// The whitespace character.
    /// </summary>
    public const char Whitespace = ' ';

    /// <summary>
    /// The builder is used to assemble each sentence.
    /// </summary>
    private readonly StringBuilder _builder = new();

    /// <summary>
    /// The end list of statements.
    /// </summary>
    private readonly List<SyntaxStatement> _statements = new();

    /// <summary>
    /// This variable determines if the parsing routine is in a middle of statement.
    /// </summary>
    private bool _currentStatement;

    /// <summary>
    /// This variable determines if the parsing routine is in a middle of quote.
    /// </summary>
    private bool _currentQuote;

    /// <summary>
    /// This variable controls if the next character needs to be escaped (ignore its special usage
    /// and include it in the statement).
    /// </summary>
    private bool _currentEscape;

    /// <summary>
    /// This variable controls if the current character should not be included into the statement.
    /// </summary>
    private bool _currentBlock;

    /// <summary>
    /// This variable controls if the last statement was quoted.
    /// </summary>
    private bool _wasQuoted;

    private void EndOfQuote()
    {
        _currentQuote = false;

        EndOfStatement();
    }

    private void SkipStatement()
    {
        _currentStatement = false;
        _statements.Add(new SyntaxStatement(_builder.ToString(), _wasQuoted));

        if (_wasQuoted)
        {
            _wasQuoted = false;
        }
        _builder.Clear();
    }

    private void EndOfStatement()
    {
        SkipStatement();
        _currentBlock = true;
    }

    /// <summary>
    /// Parses the statements from the specified lines.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>The statements parsed from the line.</returns>
    public IReadOnlyList<SyntaxStatement> ParseLine(ReadOnlySpan<char> line)
    {
        if (line.IsEmpty || line.IsWhiteSpace())
        {
            return Array.Empty<SyntaxStatement>();
        }

        // Go through every character.
        for (int i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            // Determine this at begin of statement
            if (!_currentStatement)
            {
                // It is double quote?
                if (ch == DoubleQuote)
                {
                    if (_currentQuote)
                    {
                        // If currently in quote, closes the quote.

                        EndOfQuote();
                        continue;
                    }
                    else
                    {
                        // If not, begins the quote.

                        _currentQuote = true;
                        continue;
                    }
                }

                _currentStatement = true;
            }

            // Middle of statement.

            if (ch == DoubleQuote && !_currentQuote)
            {
                SkipStatement();
                _currentQuote = true;
                continue;
            }

            if (!_currentEscape)
            {
                // Escape character.
                if (ch == EscapeChar && _currentQuote)
                {
                    _currentEscape = true;
                    continue;
                }

                if (_currentQuote && ch == DoubleQuote)
                {
                    _currentQuote = false;
                    _wasQuoted = true;
                    _currentBlock = true;
                }
            }

            // The end of statement without a quote.
            if (!_currentQuote
                && _currentStatement
                && ch == Whitespace)
            {
                EndOfStatement();
            }

            if (!_currentBlock)
            {
                _builder.Append(ch);
            }
            else
            {
                _currentBlock = false;
            }

            // The end of the string.
            if (line.Length <= i + 1)
            {
                if (_currentQuote)
                {
                    throw new FormatException("Quoted string never ends.");
                }

                if (_currentStatement)
                {
                    // End of statement now.

                    EndOfStatement();
                }
            }
        }
        
        return _statements.AsReadOnly();
    }
}
