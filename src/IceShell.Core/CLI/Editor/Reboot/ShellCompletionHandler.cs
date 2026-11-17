// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.CLI.Editor.Reboot;

using IceShell.Core.Commands.Simple;
using IceShell.Core.Syntax;
using NexusKrop.IceShell.Core.Completion.Cache;
using ReadLineReboot;

internal class RebootAutoCompleteHandler : IAutoCompleteHandler
{
    internal RebootAutoCompleteHandler(ISimpleCommandDispatcher manager, DirCache cache)
    {
        _manager = manager;
        _cache = cache;
    }

    private readonly ISimpleCommandDispatcher _manager;
    private readonly DirCache _cache;

    public char[] Separators { get; set; } = new char[] { ' ', '.', '\\' };

    public string[] GetSuggestions(string text, int index)
    {
        var parser = new CommandWordBreaker();
        var statements = parser.RunParse(text);

        if (statements.Count == 1)
        {
            var commands = CompleteCommandNames(statements[0].Content);

            return commands?.Length != 0 ? _cache.Complete(text) : commands;
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

        var list = new List<string>();

        foreach (var command in _manager.EnumerateCommandNames()
                     .Concat(_manager.EnumerateAliases()))
        {
            if (command.StartsWith(begin, StringComparison.OrdinalIgnoreCase)
                && !list.Contains(command, StringComparer.OrdinalIgnoreCase))
            {
                list.Add(command);
            }
        }

        return list.ToArray();
    }
}
