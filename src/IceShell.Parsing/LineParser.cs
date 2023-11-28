// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using IceShell.Parsing.Util;

/// <summary>
/// Provides services for extracting statements from a line.
/// </summary>
[Obsolete("Use IceShell.Core.Syntax.CommandWordBreaker instead.")]
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
    private readonly List<SyntaxWord> _statements = new();

    /// <summary>
    /// This variable determines if the parsing routine is in a middle of statement.
    /// </summary>
    private bool _currentStatement;

    /// <summary>
    /// This variable determines if the parsing routine is in a middle of quote.
    /// </summary>
    private WordQuoteType _currentQuoteType;

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
    /// This variable stores the beginning of the current statement.
    /// </summary>
    private int _currentBeginStatement = -1;

    /// <summary>
    /// This variable stores the last quote type.
    /// </summary>
    private WordQuoteType _wasQuoteType;

    private bool IsCurrentQuote() => _currentQuoteType != WordQuoteType.None;
    
    private void EndOfQuote()
    {
        _currentQuoteType = WordQuoteType.None;

        EndOfStatement();
    }

    private void SkipStatement()
    {
        _currentStatement = false;
        _statements.Add(new SyntaxWord(_builder.ToString(), _wasQuoteType, _currentBeginStatement));
        _wasQuoteType = WordQuoteType.None;

        _currentBeginStatement = -1;
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
    public IReadOnlyList<SyntaxWord> ParseLine(ReadOnlySpan<char> line)
    {
        if (line.IsEmpty || line.IsWhiteSpace())
        {
            return Array.Empty<SyntaxWord>();
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
                    if (_currentQuoteType != WordQuoteType.None)
                    {
                        // If currently in quote, closes the quote.

                        EndOfQuote();
                    }
                    else
                    {
                        // If not, begins the quote.
                        _currentQuoteType = WordQuoteType.Quotes;
                        _currentBeginStatement = i - 1;
                    }
                    
                    continue;
                }

                _currentBeginStatement = i;
                _currentStatement = true;
            }

            // Middle of statement.

            if (ch == DoubleQuote && _currentQuoteType != WordQuoteType.Quotes)
            {
                SkipStatement();
                _currentQuoteType = WordQuoteType.Quotes;
                _currentBeginStatement = i - 1;
                continue;
            }

            if (!_currentEscape)
            {
                // Escape character.
                if (ch == EscapeChar && _currentQuoteType != WordQuoteType.None)
                {
                    _currentEscape = true;
                    continue;
                }

                if (IsCurrentQuote() && ch == DoubleQuote)
                {
                    _currentQuoteType = WordQuoteType.None;
                    _wasQuoteType = WordQuoteType.Quotes;
                    _currentBlock = true;
                }
            }

            // The end of statement without a quote.
            if (!IsCurrentQuote()
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
                if (IsCurrentQuote())
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
