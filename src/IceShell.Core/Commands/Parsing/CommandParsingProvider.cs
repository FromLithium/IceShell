// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Parsing;

using System.Text;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using IceShell.Parsing;

/// <summary>
/// Provides registry command parsing services.
/// </summary>
public class CommandParsingProvider
{
    private readonly CommandRegistryEntry _definition;
    private readonly SyntaxCommandInvocation _command;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgument"/> command.
    /// </summary>
    /// <param name="command">The command syntax to parse.</param>
    /// <param name="definition">The definition of the command.</param>
    public CommandParsingProvider(SyntaxCommandInvocation command, CommandRegistryEntry definition)
    {
        _command = command;
        _definition = definition;
    }

    /// <summary>
    /// Parses all value and option arguments.
    /// </summary>
    /// <returns>The parse result.</returns>
    public CommandArgumentResult Parse()
    {
        var result = new CommandArgumentResult();
        
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

    private void ParseOptions(CommandArgumentResult result)
    {
        foreach (var option in _command.Options)
        {
            if (!_definition.Options.TryGetValue(option.Identifier, out var optionDef))
            {
                throw ExceptionHelper.WithName(LangMessage.Get("argument_no_such_option"), option.Identifier);
            }

            if (optionDef.IsSwitch)
            {
                if (option.Value != null)
                {
                    throw ExceptionHelper.WithName(LangMessage.Get("argument_option_no_value"), option.Identifier);
                }
                
                result.AddSwitch(optionDef, true);
            }
            else
            {
                if (option.Value == null)
                {
                    throw ExceptionHelper.WithName(LangMessage.Get("argument_value_required"), option.Identifier);
                }
                
                result.AddValueOption(optionDef, option.Value);
            }
        }
    }

    private void ParseVariableValues(CommandArgumentResult result)
    {
        foreach (var value in _command.Values)
        {
            result.AddVariable(value.Content);
        }
    }

    private void ParseStandardValues(CommandArgumentResult result)
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
                result.AddValue(valueDef, x.Content);

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
            result.AddValue(valueDef, sb.ToString());
        }
    }
}