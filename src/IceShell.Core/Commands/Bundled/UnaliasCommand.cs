// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Simple;
using IceShell.Parsing;

/// <summary>
/// Defines a command that removes command aliases.
/// </summary>
public static class UnaliasCommand
{
    private const string CommandName = "unalias";

    /// <summary>
    /// Registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(ISimpleCommandDispatcher dispatcher)
    {
        dispatcher.Register(CommandName, Execute);
    }

    private static int Execute(IShell shell, ICommandDocument document, IReadOnlyList<SyntaxWord> args)
    {
        if (args.Count == 0)
        {
            Console.Error.WriteLine(LangMessage.Get("shell.unalias_requires_name"));
            return 1;
        }

        var result = 0;

        foreach (var arg in args)
        {
            if (shell.Dispatcher.RemoveAlias(arg.Content))
            {
                continue;
            }

            Console.Error.WriteLine(LangMessage.GetFormat("shell.alias_not_found", arg.Content));
            result = 1;
        }

        return result;
    }
}
