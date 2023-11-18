// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using IceShell.Parsing;

/// <summary>
/// Represents context for command execution events, either for a single triggering,
/// a single shell, or a single batch file.
/// </summary>
public record ExecutionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
    /// </summary>
    /// <param name="readFrom">The text reader to read results of the last command from.</param>
    /// <param name="nextAction">The next action.</param>
    public ExecutionContext(TextReader? readFrom, SyntaxNextAction nextAction = SyntaxNextAction.None)
    {
        Retrieval = readFrom;
        NextAction = nextAction;
    }

    /// <summary>
    /// Gets or sets the text reader to read the result of the last command from.
    /// </summary>
    public TextReader? Retrieval { get; set; }

    /// <summary>
    /// Gets the next action.
    /// </summary>
    public SyntaxNextAction NextAction { get; }

    /// <summary>
    /// An empty <see cref="ExecutionContext"/>.
    /// </summary>
    public static readonly ExecutionContext Default = new(null, SyntaxNextAction.None);
}
