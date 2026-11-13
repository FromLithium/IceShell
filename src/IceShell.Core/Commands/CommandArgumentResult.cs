// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using System.Collections.ObjectModel;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;

/// <summary>
/// Provides the result of a complete command argument parsing.
/// </summary>
public class CommandArgumentResult
{
    private readonly Dictionary<char, bool> _switches = new();
    private readonly Dictionary<char, string> _valueOptions = new();
    private readonly Dictionary<string, string> _values = new();
    private readonly List<string> _variableValues = new();
    private readonly List<SyntaxWord> _words = new();
    private bool _final;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgumentResult"/> class.
    /// </summary>
    public CommandArgumentResult()
    {
        Switches = new ReadOnlyDictionary<char, bool>(_switches);
        ValueOptions = new ReadOnlyDictionary<char, string>(_valueOptions);
        Values = new ReadOnlyDictionary<string, string>(_values);
        VariableValues = _variableValues;
        OriginalWords = _words;
    }
    
    /// <summary>The switches.</summary>
    public IReadOnlyDictionary<char, bool> Switches { get; }

    /// <summary>The value options.</summary>
    public IReadOnlyDictionary<char, string> ValueOptions { get; }

    /// <summary>The values.</summary>
    public IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>
    /// Gets the variable values.
    /// </summary>
    [Obsolete("Use OriginalWords instead.")]
    public IReadOnlyList<string> VariableValues { get; }
    
    /// <summary>
    /// Gets the split up words in the original command line.
    /// </summary>
    public IReadOnlyList<SyntaxWord> OriginalWords { get; }
    
    /// <summary>
    /// Gets a value indicating whether the help information is requested, rather than an execution of the command.
    /// </summary>
    public bool IsHelpInformationRequested { get; private set; }

    /// <summary>
    /// Adds a switch to the parsing result.
    /// </summary>
    /// <param name="id">The option definition.</param>
    /// <param name="value">The value.</param>
    public void AddSwitch(char id, bool value = true)
    {
        ThrowIfFinal();
        
        _switches.Add(id, value);
    }

    /// <summary>
    /// Adds a value option to the parsing result.
    /// </summary>
    /// <param name="id">The option definition.</param>
    /// <param name="value">The value.</param>
    public void AddValueOption(char id, string value)
    {
        ThrowIfFinal();
        
        _valueOptions.Add(id, value);
    }

    /// <summary>
    /// Adds a value argument to the parsing result.
    /// </summary>
    /// <param name="info">The parameter.</param>
    /// <param name="value">The value.</param>
    public void AddValue(CommandValueInfo info, string value)
    {
        ThrowIfFinal();
        
        _values.Add(info.ReferenceName, value);
    }

    /// <summary>
    /// Adds a variable value to the parsing result.
    /// </summary>
    /// <param name="value">The value to add.</param>
    [Obsolete("Use AddWord instead.")]
    public void AddVariable(string value)
    {
        ThrowIfFinal();
        
        _variableValues.Add(value);
    }

    /// <summary>
    /// Adds a word from the original command line to the parsing result.
    /// </summary>
    /// <param name="word">The word to add.</param>
    public void AddWord(SyntaxWord word)
    {
        ThrowIfFinal();
        
        _words.Add(word);
    }

    /// <summary>
    /// Renders this instance immutable after this method call. Any further attempts will result in exceptions.
    /// </summary>
    public void MakeFinal()
    {
        _final = true;
        
        _variableValues.AddRange(_words.Select(x => x.Content));
    }
    
    private void ThrowIfFinal()
    {
        if (_final)
        {
            throw new InvalidOperationException("The parsing result is already finalised.");
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandArgumentResult"/> that requests the help information about a command
    /// rather than the execution of such command.
    /// </summary>
    /// <returns>The created <see cref="CommandArgumentResult"/>.</returns>
    public static CommandArgumentResult HelpRequested()
    {
        var result = new CommandArgumentResult()
        {
            IsHelpInformationRequested = true
        };
        result.MakeFinal();
        return result;
    }
}