// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using System.Collections.ObjectModel;

/// <summary>
/// Describes a command.
/// </summary>
public class CommandRegistryEntry
{
    private readonly List<CommandValueInfo> _valueList = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRegistryEntry"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the command.</param>
    public CommandRegistryEntry(string identifier)
    {
        Identifier = identifier;

        Values = new ReadOnlyCollection<CommandValueInfo>(_valueList);
    }

    public IDictionary<char, CommandOptionInfo> Options { get; } = new Dictionary<char, CommandOptionInfo>();
    public IReadOnlyList<CommandValueInfo> Values { get; }
    
    /// <summary>
    /// Gets or sets the identifier of this command.
    /// </summary>
    public string Identifier { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the usage string of this command shall be a set value
    /// rather than automatically generated from definitions.
    /// </summary>
    public string? CustomUsage { get; set; }
    
    /// <summary>
    /// Gets or sets a list of platforms that this command supports.
    /// </summary>
    public ICollection<string>? Platforms { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating that whether this command has a variable value parameter format
    /// and the parsing of values should be handled by the command implementation.
    /// </summary>
    public bool VariableValues { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating that whether the last value argument ignore white space separation
    /// without being quoted.
    /// </summary>
    public bool GreedyString { get; set; }

    /// <summary>
    /// Registers a new value parameter.
    /// </summary>
    /// <param name="info">The value info.</param>
    public void AddValue(CommandValueInfo info)
    {
        if (info.Position < 0)
        {
            _valueList.Add(info);
            return;
        }
        
        _valueList.EnsureCapacity(info.Position);
        _valueList.Insert(info.Position, info);
    }

    /// <summary>
    /// Creates a new value parameter and registers it to this entry.
    /// </summary>
    /// <param name="referenceName">The reference name of the value parameter.</param>
    /// <param name="position">The position of the value parameter.</param>
    /// <param name="required">If <see langword="true"/>, the value is required.</param>
    /// <returns>The created command value parameter definition.</returns>
    public CommandValueInfo AddValue(string referenceName, int position, bool required = false)
    {
        var result = new CommandValueInfo(referenceName, position, required);
        AddValue(result);
        return result;
    }

    /// <summary>
    /// Registers a new option parameter.
    /// </summary>
    /// <param name="info">The option info.</param>
    public void AddOption(CommandOptionInfo info)
    {
        Options[info.Identifier] = info;
    }

    /// <summary>
    /// Creates a new option parameter and registers it to this entry.
    /// </summary>
    /// <param name="identifier">The identifier of the option.</param>
    /// <param name="isSwitch">If <see langword="true"/>, this option represents a boolean value which represents whether the option was specified.</param>
    /// <returns>The created command option parameter.</returns>
    public CommandOptionInfo AddOption(char identifier, bool isSwitch)
    {
        var result = new CommandOptionInfo(identifier, isSwitch);
        AddOption(result);
        return result;
    }
}