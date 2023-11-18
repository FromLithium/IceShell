// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Completion;

using global::IceShell.Core.Api;
using global::IceShell.Core.Api.Commands;
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
        var commands = CompleteCommand(text);

        if (commands?.IsEmpty() != false)
        {
            return _cache.Complete(text);
        }

        return commands;
    }

    /// <summary>
    /// Returns a list of the command names that begins with the specified characters.
    /// </summary>
    /// <param name="begin">The characters to search for completion.</param>
    /// <returns>The list of command names.</returns>
    public string[] CompleteCommand(string begin)
    {
        if (string.IsNullOrWhiteSpace(begin))
        {
            return Array.Empty<string>();
        }

        var list = new List<string>(_manager.AliasCount);

        foreach (var command in _manager.EnumerateAliases())
        {
            if (command.Equals(begin))
            {
                list.Add(command);
                break;
            }

            if (command.StartsWith(begin))
            {
                list.Add(command);
            }
        }

        return list.ToArray();
    }
}
