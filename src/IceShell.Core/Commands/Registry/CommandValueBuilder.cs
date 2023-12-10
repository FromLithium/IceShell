// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

/// <summary>
/// Provides utilities to construct and register command value parameters. This class cannot be inherited.
/// </summary>
public sealed class CommandValueBuilder
{
    private readonly CommandBuilder _parent;
    private readonly string _referenceName;
    private bool _required;
    private bool _noExpandVariables;
    private int _position = -1;
    
    internal CommandValueBuilder(CommandBuilder parent, string referenceName)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceName);
        
        _parent = parent;
        _referenceName = referenceName;
    }

    /// <summary>
    /// Registers this value parameter as required.
    /// </summary>
    /// <returns>This instance, for chaining.</returns>
    public CommandValueBuilder Required()
    {
        _required = true;
        return this;
    }

    /// <summary>
    /// Registers this value parameter so that variable notations in this parameter will not be parsed
    /// and expanded.
    /// </summary>
    /// <returns>This instance, for chaining.</returns>
    public CommandValueBuilder NoExpandVariables()
    {
        _noExpandVariables = true;
        return this;
    }

    /// <summary>
    /// Registers the position of this value parameter.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>This instance, for chaining.</returns>
    public CommandValueBuilder Position(int position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Constructs the value parameter definition and registers it to the command.
    /// </summary>
    /// <returns>The parent command builder.</returns>
    /// <exception cref="InvalidOperationException">Null, empty or invalid reference name.</exception>
    public CommandBuilder Build()
    {
        if (string.IsNullOrWhiteSpace(_referenceName))
        {
            throw new InvalidOperationException("Null, empty or invalid reference name.");
        }
        
        _parent.ValueParameterInternal(_referenceName, _position, _required, _noExpandVariables);
        return _parent;
    }
}