// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Parsing;

/// <summary>
/// Represents a command execution context.
/// </summary>
public struct CommandContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandContext"/> structure.
    /// </summary>
    /// <param name="arguments">The parsed arguments.</param>
    /// <param name="nextAction">The next action.</param>
    /// <param name="retrieval">The retrieval pipe stream.</param>
    public CommandContext(CommandArgumentResult arguments, SyntaxNextAction nextAction, TextReader? retrieval = null)
    {
        Arguments = arguments;
        NextAction = nextAction;
        Retrieval = retrieval;
    }
    
    /// <summary>
    /// Gets the next action.
    /// </summary>
    public SyntaxNextAction NextAction { get; }
    
    /// <summary>
    /// Gets or sets the text reader to read the result of the last command from.
    /// </summary>
    public TextReader? Retrieval { get; }
    
    /// <summary>
    /// Gets the arguments supplied to this command.
    /// </summary>
    public CommandArgumentResult Arguments { get; }

    /// <summary>
    /// Gets the state of the specified switch option.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <returns><see langword="true" /> if the switch is set; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentException">The supplied option is not a switch.</exception>
    public bool GetSwitch(CommandOptionInfo option)
    {
        if (!option.IsSwitch)
        {
            throw new ArgumentException("The supplied option is not a switch.", nameof(option));
        }

        return Arguments.Switches.ContainsKey(option) && Arguments.Switches[option];
    }
}