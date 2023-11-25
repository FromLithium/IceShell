namespace IceShell.Parsing;

/// <summary>
/// Represents a reference to external command.
/// </summary>
public record struct ExternalCommandRef
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalCommandRef"/> class.
    /// </summary>
    /// <param name="refName">The reference name of the command.</param>
    /// <param name="arguments">The arguments to pass to the command.</param>
    public ExternalCommandRef(string? refName, IEnumerable<SyntaxWord>? arguments)
    {
        ReferenceName = refName;
        Arguments = arguments;
    }

    /// <summary>
    /// Gets or sets the reference name of the command.
    /// </summary>
    public string? ReferenceName { get; set; }

    /// <summary>
    /// Gets or sets the arguments to pass to the external command.
    /// </summary>
    public IEnumerable<SyntaxWord>? Arguments { get; set; }

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <returns><see langword="true"/> if this instance is empty; otherwise, <see langword="false"/>.</returns>
    public readonly bool IsEmpty()
    {
        return string.IsNullOrEmpty(ReferenceName);
    }
}
