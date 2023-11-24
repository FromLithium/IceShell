// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using System.Collections.ObjectModel;
using IceShell.Core.Commands.Registry;
using NexusKrop.IceCube.Collections;

/// <summary>
/// Provides the result of a complete command argument parsing.
/// </summary>
public class CommandArgumentResult
{
    private readonly Dictionary<char, bool> _switches = new();
    private readonly Dictionary<char, string> _valueOptions = new();
    private readonly Dictionary<string, string> _values = new();
    private readonly List<string> _variableValues = new();
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
    public IReadOnlyList<string> VariableValues { get; }

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
    public void AddVariable(string value)
    {
        ThrowIfFinal();
        
        _variableValues.Add(value);
    }

    /// <summary>
    /// Renders this instance immutable after this method call. Any further attempts will result in exceptions.
    /// </summary>
    public void MakeFinal()
    {
        _final = true;
    }
    
    private void ThrowIfFinal()
    {
        if (_final)
        {
            throw new InvalidOperationException("The parsing result is already finalised.");
        }
    }
}