// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

using IceShell.Core.Api.Commands;

/// <summary>
/// Provides chaining (fluent) syntax for constructing and registering commands.
/// </summary>
public class CommandBuilder
{
    private readonly CommandRegistryEntry _entry;
    private readonly CommandDelegate _routine;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBuilder"/> class.
    /// </summary>
    /// <param name="id">The identifier of the command.</param>
    /// <param name="routine">The command processing routine.</param>
    public CommandBuilder(string id, CommandDelegate routine)
    {
        _entry = new CommandRegistryEntry(id);
        _routine = routine;
    }

    /// <summary>
    /// Registers a switch option to the command.
    /// </summary>
    /// <param name="id">The identifier character of the switch option.</param>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder SwitchOption(char id)
    {
        _entry.AddOption(id, true);
        return this;
    }

    /// <summary>
    /// Registers a value option to the command.
    /// </summary>
    /// <param name="id">The identifier character of the value option.</param>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder ValueOption(char id)
    {
        _entry.AddOption(id, false);
        return this;
    }

    internal void ValueParameterInternal(string refName, int position, bool required, bool noExpandVariables)
    {
        _entry.AddValue(new CommandValueInfo(refName, position, required)
        {
            NoExpandVariables = noExpandVariables
        });
    }

    /// <summary>
    /// Registers a value parameter to the command.
    /// </summary>
    /// <param name="refName">The reference name of the value parameter. Used to cite the value parameter.</param>
    /// <param name="position">The position of the value parameter in the command.</param>
    /// <param name="required">If <see langword="true"/>, any attempt to invoke the command without this parameter specified will be rejected with an error.</param>
    /// <returns>This instance, for chaining.</returns>
    [Obsolete("Use ValueParameter(string) instead.")]
    public CommandBuilder ValueParameter(string refName, int position, bool required = false)
    {
        ValueParameterInternal(refName, position, required, false);
        return this;
    }

    /// <summary>
    /// Construct and register a value parameter to the command.
    /// </summary>
    /// <param name="refName">The reference name.</param>
    /// <returns>An instance of <see cref="CommandValueBuilder"/> to construct and register the command.</returns>
    public CommandValueBuilder ValueParameter(string refName)
    {
        return new CommandValueBuilder(this, refName);
    }

    /// <summary>
    /// Registers the command as a greedy string command.
    /// </summary>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder GreedyString()
    {
        _entry.GreedyString = true;
        return this;
    }

    /// <summary>
    /// Registers the command as a variable values command.
    /// </summary>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder VariableValues()
    {
        _entry.VariableValues = true;
        return this;
    }

    /// <summary>
    /// Registers a custom usage line to the command.
    /// </summary>
    /// <param name="usage">The custom usage line.</param>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder CustomUsage(string usage)
    {
        _entry.CustomUsage = usage;
        return this;
    }

    /// <summary>
    /// Registers this command so that variable notations in the arguments are not expanded for this command
    /// even if <see cref="IceShell.Core.Settings.ShellSettings.ExpandVariablesForAllValues"/> is set to <see langword="true"/>.
    /// </summary>
    /// <returns>This instance, for chaining.</returns>
    public CommandBuilder NoParseVariables()
    {
        _entry.NoParseVariables = true;
        return this;
    }
    
    /// <summary>
    /// Constructs the command and register it to the dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register the command to.</param>
    public void Build(IRegistryCommandDispatcher dispatcher)
    {
        dispatcher.Register(_entry, _routine);
    }

    /// <summary>
    /// Constructs the command and returns the results.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="routine">The command routine.</param>
    public void Build(out CommandRegistryEntry entry, out CommandDelegate routine)
    {
        entry = _entry;
        routine = _routine;
    }
}