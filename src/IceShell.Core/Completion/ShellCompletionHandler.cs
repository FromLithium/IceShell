// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Completion;

using global::IceShell.Core.Api;
using global::IceShell.Core.Api.Commands;
using global::IceShell.Core.Syntax;
using global::IceShell.Parsing;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.Completion.Cache;
using ReadLineReboot;

internal class ShellCompletionHandler : IAutoCompleteHandler
{
    internal ShellCompletionHandler(IRegistryCommandDispatcher manager, DirCache cache)
    {
        _manager = manager;
        _cache = cache;
    }

    private readonly IRegistryCommandDispatcher _manager;
    private readonly DirCache _cache;

    public char[] Separators { get; set; } = new char[] { ' ', '.', '\\' };

    public string[] GetSuggestions(string text, int index)
    {
        var parser = new CommandWordBreaker();
        var statements = parser.RunParse(text);

        if (statements.Count == 1)
        {
            var commands = CompleteCommandNames(statements[0].Content);

            return commands?.IsEmpty() != false ? _cache.Complete(text) : commands;
        }
        else
        {
            // TODO implement this
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Returns a list of the command names that begins with the specified characters.
    /// </summary>
    /// <param name="begin">The characters to search for completion.</param>
    /// <returns>The list of command names.</returns>
    private string[] CompleteCommandNames(string begin)
    {
        if (string.IsNullOrWhiteSpace(begin))
        {
            return Array.Empty<string>();
        }

        var list = new List<string>(_manager.AliasCount);

        foreach (var command in _manager.EnumerateAliases())
        {
            var upper = begin.ToUpperInvariant();
            
            if (command.Equals(upper))
            {
                list.Add(command);
                break;
            }

            if (command.StartsWith(upper))
            {
                list.Add(command);
            }
        }

        return list.ToArray();
    }
}
