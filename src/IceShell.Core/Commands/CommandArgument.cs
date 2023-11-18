// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands;

using IceShell.Core.CLI.Languages;
using IceShell.Core.Exceptions;
using IceShell.Parsing;
using NexusKrop.IceShell.Core.Commands.Complex;
using System.Linq;
using System.Text;

/// <summary>
/// Provides model and parsing services for the modern command parsing routine.
/// </summary>
[Obsolete("Use related services via CommandParsingProvider.")]
public class CommandArgument
{
    private readonly SyntaxCommandInvocation _command;
    private readonly CommandDefinition _definition;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgument"/> command.
    /// </summary>
    /// <param name="command">The command syntax to parse.</param>
    /// <param name="definition">The definition of the command.</param>
    public CommandArgument(SyntaxCommandInvocation command, CommandDefinition definition)
    {
        _command = command;
        _definition = definition;
    }

    /// <summary>
    /// Parses all value and option arguments.
    /// </summary>
    /// <returns>The parse result.</returns>
    public ArgumentParseResult Parse()
    {
        var result = new ArgumentParseResult();

        if (_definition.VariableValues)
        {
            ParseVariableValues(result);
        }
        else
        {
            ParseStandardValues(result);
        }

        ParseOptions(result);

        return result;
    }

    private void ParseOptions(ArgumentParseResult result)
    {
        foreach (var option in _command.Options)
        {
            if (!_definition.Options.TryGetValue(option.Identifier, out var optionDef))
            {
                throw ExceptionHelper.WithName(LangMessage.Get("argument_no_such_option"), option.Identifier);
            }

            if (!optionDef.HasValue && option.Value != null)
            {
                throw ExceptionHelper.WithName(LangMessage.Get("argument_option_no_value"), option.Identifier);
            }

            if (optionDef.HasValue && option.Value == null)
            {
                throw ExceptionHelper.WithName(LangMessage.Get("argument_value_required"), option.Identifier);
            }

            result.Option(optionDef, option.Value);
        }
    }

    private void ParseVariableValues(ArgumentParseResult result)
    {
        foreach (var value in _command.Values)
        {
            result.VariableValues.Add(value.Content);
        }
    }

    private void ParseStandardValues(ArgumentParseResult result)
    {
        var requiredArgCount = _definition.Values.Count(x => x.Required);

        if (_command.Values.Count < requiredArgCount)
        {
            throw new CommandFormatException(LangMessage.MsgTooFewArguments(_command.Values.Count, requiredArgCount));
        }

        var index = 0;
        var startGreedy = false;

        foreach (var x in _command.Values)
        {
            // Check if too many arguments
            if (!_definition.GreedyString && index >= _definition.Values.Count)
            {
                throw new CommandFormatException(LangMessage.MsgTooManyArguments(index + 1, _definition.Values.Count));
            }

            if (_definition.GreedyString && index == _definition.Values.Count - 1)
            {
                startGreedy = true;
                break;
            }
            else
            {
                var valueDef = _definition.Values[index];
                result.Value(valueDef, x.Content);

                index++;
            }
        }

        if (startGreedy)
        {
            var sb = new StringBuilder();
            var first = true;
            var skipId = 0;

            foreach (var value in _command.Values)
            {
                if (skipId < index)
                {
                    skipId++;
                    continue;
                }

                if (!first)
                {
                    sb.Append(' ');
                }

                first = false;

                sb.Append(value.Content);
            }

            var valueDef = _definition.Values[index];
            result.Value(valueDef, sb.ToString());
        }
    }
}
