// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing;

using JetBrains.Annotations;

/// <summary>
/// Represents a word in a command line, the basis for all command arguments (options, values, etc.).
/// </summary>
public record SyntaxWord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxWord"/> class.
    /// </summary>
    /// <param name="content">The string content.</param>
    /// <param name="wasQuoted">Whether the statement was specified via a quoted string.</param>
    [PublicAPI]
    public SyntaxWord(string content, bool wasQuoted) : this(content, wasQuoted, -1)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxWord"/> class.
    /// </summary>
    /// <param name="content">The string content.</param>
    /// <param name="wasQuoted">Whether the statement was specified via a quoted string.</param>
    /// <param name="startsFrom">The position immediately before the first character of this word from the original string.</param>
    public SyntaxWord(string content, bool wasQuoted, int startsFrom)
    {
        Content = content;
        WasQuoted = wasQuoted;
        StartsFrom = startsFrom;
    }

    /// <summary>
    /// Gets or sets the text content of this word.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Gets or sets whether this word was quoted to contain multiple words.
    /// </summary>
    public bool WasQuoted { get; }
    
    /// <summary>
    /// Gets or sets the position right before the first character of this word in the original string.
    /// </summary>
    /// <value>
    /// If is positive value, the position right before the first character of this word; otherwise if negative value, then the position is unspecified.
    /// </value>
    public int StartsFrom { get; }

    /// <summary>
    /// Creates a list of unquoted statements with the specified contents.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <returns>A new list of unquoted statements.</returns>
    public static IReadOnlyList<SyntaxWord> Of(params string[] contents)
    {
        var list = new List<SyntaxWord>();

        foreach (var item in contents)
        {
            list.Add(new(item, false, -1));
        }

        return list.AsReadOnly();
    }
}
