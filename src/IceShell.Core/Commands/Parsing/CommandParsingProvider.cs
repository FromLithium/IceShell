// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Parsing;

using System.Text;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using IceShell.Parsing;
using NexusKrop.IceCube.Util.Enumerables;

/// <summary>
/// Provides registry command parsing services.
/// </summary>
public class CommandParsingProvider
{
    private readonly CommandRegistryEntry _definition;
    private readonly SyntaxCommandInvocation _command;
    private readonly bool _expandAllVariables;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgument"/> command.
    /// </summary>
    /// <param name="command">The command syntax to parse.</param>
    /// <param name="definition">The definition of the command.</param>
    /// <param name="expandAllVariables">If <see langword="true"/>, then all variables are expanded accordingly.</param>
    public CommandParsingProvider(SyntaxCommandInvocation command, CommandRegistryEntry definition,
        bool expandAllVariables = false)
    {
        _command = command;
        _definition = definition;
        _expandAllVariables = expandAllVariables;
    }

    /// <summary>
    /// Parses all value and option arguments.
    /// </summary>
    /// <param name="allSwitches">The switches that will be checked for.</param>
    /// <returns>The parse result.</returns>
    public CommandArgumentResult Parse(ISet<char>? allSwitches)
    {
        var result = new CommandArgumentResult();

        if (!_definition.Options.ContainsKey('?')
            && _command.Options.Contains(new SyntaxOption('?', null)))
        {
            return CommandArgumentResult.HelpRequested();
        }
        
        if (_definition.VariableValues)
        {
            AddOriginalWords(result);
        }
        else
        {
            ParseStandardValues(result);
        }

        ParseOptions(result, allSwitches);

        result.MakeFinal();
        return result;
    }

    private void ParseOptions(CommandArgumentResult result, ISet<char>? allSwitches)
    {
        var setSwitches = new HashSet<char>();
        
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
                
                result.AddSwitch(optionDef.Identifier, true);
                setSwitches.Add(optionDef.Identifier);
            }
            else
            {
                if (option.Value == null)
                {
                    throw ExceptionHelper.WithName(LangMessage.Get("argument_value_required"), option.Identifier);
                }
                
                result.AddValueOption(optionDef.Identifier, option.Value);
            }
        }

        allSwitches?.Where(x => !setSwitches.Contains(x))
            .ForEach(x => result.AddSwitch(x, false));
    }
    
    private void AddOriginalWords(CommandArgumentResult result)
    {
        _command.Values.ForEach(result.AddWord);
    }

    private void ParseStandardValues(CommandArgumentResult result)
    {
        AddOriginalWords(result);
        
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

            var actualStr = x.Content;
            
            // Configuration option shall control whether to expand all variables.
            if (_expandAllVariables)
            {
                actualStr = PathSearcher.ExpandVariables(actualStr, true);
            }

            // If greedy string is on and we are at the last value as defined...
            if (_definition.GreedyString && index == _definition.Values.Count - 1)
            {
                startGreedy = true;
                break;
            }
            else
            {
                var valueDef = _definition.Values[index];
                result.AddValue(valueDef, actualStr);

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

            var actualStr = sb.ToString();
            
            if (_expandAllVariables)
            {
                actualStr = PathSearcher.ExpandVariables(actualStr, true);
            }
            
            result.AddValue(valueDef, actualStr);
        }
    }
}