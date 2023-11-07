// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using IceShell.Core.CLI.Languages;
using NexusKrop.IceShell.Core.Commands.Complex;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Represents a model that defines a command and its parameters.
/// </summary>
/// <remarks>
/// Command definitions does not know about the name of the command, and this behaviour is intentional. Commands can have aliases,
/// and is up to the developer to determine the name of the command to display in help messages, etc.
/// </remarks>
public class CommandDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDefinition"/> class.
    /// </summary>
    /// <param name="id">The ID of the command.</param>
    public CommandDefinition(string id)
    {
        Id = id;
    }

    internal Dictionary<char, ComplexOptionDefinition> Options { get; } = new();
    internal List<ComplexValueDefinition> Values { get; } = new();

    /// <summary>
    /// Gets or sets the ID of the command.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets whether the command uses variable values.
    /// </summary>
    public bool VariableValues { get; set; }

    /// <summary>
    /// Gets or sets whether the last value provided will be a greedy string.
    /// </summary>
    public bool GreedyString { get; set; }

    /// <summary>
    /// Gets or sets the variable value buffer of the command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Commands with a variable amount of values cannot accept values in regular properties, instead, it requires a 
    /// list that is able to put the values into.
    /// </para>
    /// <para>
    /// Attempting to register a variable value command that does not have a variable value buffer will result in an exception. 
    /// </para>
    /// </remarks>
    public PropertyInfo? VariableValueBuffer { get; set; }

    /// <summary>
    /// Gets or sets the custom usage string of the command. It will be appended after the command name.
    /// </summary>
    public string? CustomUsage { get; set; }

    /// <summary>
    /// Prints help message of the command.
    /// </summary>
    /// <param name="cmdName">The command name to use in the help message.</param>
    [Obsolete("The user should create their own print help message function.")]
    public void PrintHelp(string cmdName)
    {
        Console.WriteLine(GetUsage(cmdName));

        var noDescText = LangMessage.Get("help_no_description");

        if (Values.Any())
        {
            Console.WriteLine();

            // Values
            Console.WriteLine("Values: ");
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (var value in Values)
            {
                grid.AddRow(value.Name, noDescText);
            }

            AnsiConsole.Write(grid);
        }

        if (Options.Any())
        {
            Console.WriteLine();

            // Options
            Console.WriteLine("Options: ");
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (var option in Options)
            {
                grid.AddRow(option.Value.HasValue ? $"/{option.Key}" : $"/{option.Key}:<value>",
                    noDescText);
            }

            AnsiConsole.Write(grid);
        }
    }

    /// <summary>
    /// Creates a string that describes this command definition in a user-friendly way.
    /// </summary>
    /// <param name="cmdName">The name of the command to include in the usage.</param>
    /// <returns>The command usage definition string.</returns>
    [Obsolete("Command should assembly their own usage string.")]
    public string GetUsage(string cmdName)
    {
        if (CustomUsage != null)
        {
            return $"{cmdName} {CustomUsage}";
        }

        var builder = new StringBuilder();
        builder.Append(cmdName).Append(' ');

        if (Options.Any())
        {
            builder.Append("[options...]");
        }

        Values.ForEach(x =>
        {
            if (x.Required)
            {
                builder.Append('<').Append(x.Name).Append('>');
            }
            else
            {
                builder.Append('[').Append(x.Name).Append(']');
            }

            builder.Append(' ');
        });

        return builder.ToString();
    }

    /// <summary>
    /// Registers a new option to this definition.
    /// </summary>
    /// <param name="name">The specifier of the option.</param>
    /// <param name="hasValue">If <see langword="true"/>, the option is a value option; otherwise, it is a switch option.</param>
    /// <param name="property">The property to bind with the option.</param>
    /// <param name="required">If <see langword="true"/>, the existence of this option will be enforced.</param>
    /// <returns>This instance (for chaining purposes).</returns>
    public CommandDefinition Option(char name, bool hasValue, PropertyInfo property, bool required = false)
    {
        Option(new(name, hasValue, property, required));

        return this;
    }

    /// <summary>
    /// Registers a new option to this definition.
    /// </summary>
    /// <param name="option">The definition of the option to register.</param>
    /// <returns>This instance (for chaining purposes).</returns>
    public CommandDefinition Option(ComplexOptionDefinition option)
    {
        Options.Add(option.ShortName, option);

        return this;
    }

    /// <summary>
    /// Registers a new value argument to this definition.
    /// </summary>
    /// <param name="definition">The definition of value argument.</param>
    /// <param name="position">The position of the value argument.</param>
    /// <returns>This <see cref="CommandDefinition"/> instance.</returns>
    /// <exception cref="ArgumentException">Required complex argument values cannot have preceding optional options.</exception>
    public CommandDefinition Value(ComplexValueDefinition definition, int position = -1)
    {
        if (Values.Any() && !Values[^1].Required && definition.Required)
        {
            throw new ArgumentException("Required complex argument values cannot have preceding optional options.");
        }

        if (position > 0)
        {
            Values.EnsureCapacity(position);
            Values.Insert(position, definition);
        }
        else
        {
            Values.Add(definition);
        }

        return this;
    }

    /// <summary>
    /// Instructs the parsing routine that checks for required arguments will be done by the command rather than
    /// the parsing routine, and the parsing routine should only check options.
    /// </summary>
    /// <returns>The current <see cref="CommandDefinition"/> instance.</returns>
    public CommandDefinition VarValues()
    {
        VariableValues = true;
        return this;
    }

    /// <summary>
    /// Instructs the parsing routine that the first variable will be a greedy string value, and all others
    /// will not be parsed.
    /// </summary>
    /// <returns>The current <see cref="CommandDefinition"/> instance.</returns>
    public CommandDefinition Greedy()
    {
        GreedyString = true;
        return this;
    }
}
