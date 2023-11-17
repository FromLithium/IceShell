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
    private readonly Dictionary<CommandOptionInfo, bool> _switches = new();
    private readonly Dictionary<CommandOptionInfo, string> _valueOptions = new();
    private readonly Dictionary<CommandValueInfo, string> _values = new();
    private readonly bool _final;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgumentResult"/> class.
    /// </summary>
    public CommandArgumentResult()
    {
        Switches = new ReadOnlyDictionary<CommandOptionInfo, bool>(_switches);
        ValueOptions = new ReadOnlyDictionary<CommandOptionInfo, string>(_valueOptions);
        Values = new ReadOnlyDictionary<CommandValueInfo, string>(_values);
        VariableValues = new List<string>();
    }
    
    /// <summary>The switches.</summary>
    public IReadOnlyDictionary<CommandOptionInfo, bool> Switches { get; }

    /// <summary>The value options.</summary>
    public IReadOnlyDictionary<CommandOptionInfo, string> ValueOptions { get; }

    /// <summary>The values.</summary>
    public IReadOnlyDictionary<CommandValueInfo, string> Values { get; }

    /// <summary>
    /// Gets the variable values.
    /// </summary>
    public IList<string> VariableValues { get; }

    /// <summary>
    /// Adds a switch to the parsing result.
    /// </summary>
    /// <param name="info">The option definition.</param>
    /// <param name="value">The value.</param>
    public void AddSwitch(CommandOptionInfo info, bool value = true)
    {
        ThrowIfFinal();
        
        _switches.Add(info, value);
    }

    /// <summary>
    /// Adds a value option to the parsing result.
    /// </summary>
    /// <param name="info">The option definition.</param>
    /// <param name="value">The value.</param>
    public void AddValueOption(CommandOptionInfo info, string value)
    {
        ThrowIfFinal();
        
        _valueOptions.Add(info, value);
    }

    /// <summary>
    /// Adds a value argument to the parsing result.
    /// </summary>
    /// <param name="info">The parameter.</param>
    /// <param name="value">The value.</param>
    public void AddValue(CommandValueInfo info, string value)
    {
        ThrowIfFinal();
        
        _values.Add(info, value);
    }

    /// <summary>
    /// Adds a variable value to the parsing result.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void AddVariable(string value)
    {
        VariableValues.Add(value);
    }
    
    private void ThrowIfFinal()
    {
        if (_final)
        {
            throw new InvalidOperationException("The parsing result is already finalised.");
        }
    }
}