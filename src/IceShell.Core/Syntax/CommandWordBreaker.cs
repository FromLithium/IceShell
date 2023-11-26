// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Syntax;

using System.Buffers;
using System.Text;
using IceShell.Parsing;
using IceShell.Parsing.Util;

/// <summary>
/// Provides a service to break words from a line.
/// </summary>
public class CommandWordBreaker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandWordBreaker"/> class.
    /// </summary>
    public CommandWordBreaker()
    {
        _wordBuilder = new StringBuilder();
        _resultList = new List<SyntaxWord>();
    }
    
    #region Constant Characters

    private const char DoubleQuote = '"';
    private const char LeftRoundBracket = '(';
    private const char RightRoundBracket = ')';
    private const char WhiteSpace = ' ';
    
    #endregion
    
    #region Control Variables

    private readonly StringBuilder _wordBuilder;
    
    /// <summary>
    /// This variable denotes whether the cursor is currently inside a statement.
    /// </summary>
    private bool _currentInsideStatement;

    /// <summary>
    /// This variable denotes the start position of the current word.
    /// </summary>
    private int _currentStatementBegin;

    /// <summary>
    /// The current cursor position.
    /// </summary>
    private int _cursor;

    /// <summary>
    /// This variable denotes whether an escape character is in progress.
    /// </summary>
    private bool _escaping;

    /// <summary>
    /// This variable denotes the quoting status of the current statement.
    /// </summary>
    private WordQuoteType _currentQuoteType;

    private readonly List<SyntaxWord> _resultList;

    /// <summary>
    /// This variable denotes if this instance should be disposed.
    /// </summary>
    private bool _final;
    
    #endregion

    public IReadOnlyList<SyntaxWord> RunParse(ReadOnlySpan<char> line)
    {
        if (_final)
        {
            throw new InvalidOperationException("Requires a new instance to parse a new line.");
        }
        
        if (line.IsEmpty || line.IsWhiteSpace())
        {
            return Array.Empty<SyntaxWord>();
        }
        
        // Go through every single character.
        for (_cursor = 0; _cursor < line.Length; _cursor++)
        {
            var current = line[_cursor];
            var next = _cursor + 1 >= line.Length ? '\0' : line[_cursor + 1];
            
            // This if-else ensures that _escaping is always set to false
            // AFTER the parsing have ran.
            if (_escaping)
            {
                RunOnce(current, next);
                _escaping = false;
            }
            else
            {
                RunOnce(current, next);
            }
        }

        // Some post cleanup work.
        PostRun();
        
        _final = true;
        return _resultList.AsReadOnly();
    }

    private void PostRun()
    {
        if (!_currentInsideStatement)
        {
            return;
        }

        if (_currentQuoteType != WordQuoteType.None)
        {
            throw new FormatException("Current quote never ends.");
        }
        
        BreakWord();
    }

    private void RunOnce(char current, char next)
    {
        if (!_currentInsideStatement && !BeforeStatement(current))
        {
            return;
        }

        if (_currentInsideStatement)
        {
            ProcessCharInStatement(current, next);
        }
    }

    private void ProcessCharInStatement(char current, char next)
    {
        // Process end of quote characters.
        if (_currentQuoteType != WordQuoteType.None
            && !_escaping)
        {
            switch (current)
            {
                // Use Right round bracket ')' to end a quoted word.
                case RightRoundBracket:
                    if (_currentQuoteType != WordQuoteType.Parentheses)
                    {
                        throw new FormatException("Using right round bracket while in non-parentheses quote!");
                    }
                    
                    BreakWord();
                    return;
                
                // Use the double quote again to end a quoted word.
                case DoubleQuote:
                    if (_currentQuoteType != WordQuoteType.Quotes)
                    {
                        throw new FormatException("Using the double quote to end quote while in a non-quote quote!");
                    }
                    
                    BreakWord();
                    return;
            }
        }

        // For non-quoted words, break word when whitespace.
        if (_currentQuoteType == WordQuoteType.None
            && current == WhiteSpace)
        {
            BreakWord();
            
            // If the next character is a quote begin character, then immediately push the cursor
            // to the next character.
            
            // If cursor will be running out then the 'next' should be 0x00 (null) which certainly is not
            // DoubleQuote or LeftRoundBracket.
            if (next is DoubleQuote or LeftRoundBracket)
            {
                // _cursor++;
            }
            return;
        }

        _wordBuilder.Append(current);
    }
    
    private bool BeforeStatement(char current)
    {
        if (!_currentInsideStatement
            && _currentQuoteType == WordQuoteType.None
            && !_escaping)
        {
            switch (current)
            {
                // Processes double quote.
                case DoubleQuote:
                    _currentQuoteType = WordQuoteType.Quotes;
                    _currentStatementBegin = _cursor;
                    _currentInsideStatement = true;
                    return false;
                // Left round bracket: Begin parentheses.
                case LeftRoundBracket:
                    _currentQuoteType = WordQuoteType.Parentheses;
                    _currentStatementBegin = _cursor;
                    _currentInsideStatement = true;
                    return false;
            }
        }

        if (!_currentInsideStatement && current != WhiteSpace)
        {
            // Starts a new unquoted statement if this is at begin of a word.
            _currentInsideStatement = true;
            _currentQuoteType = WordQuoteType.None;
            _currentStatementBegin = _cursor;
        }

        return true;
    }

    private void BreakWord()
    {
        _resultList.Add(new SyntaxWord(_wordBuilder.ToString(), _currentQuoteType, _currentStatementBegin));
        _currentQuoteType = WordQuoteType.None;
        _currentInsideStatement = false;
        _currentStatementBegin = -1;
        _wordBuilder.Clear();
    }
}