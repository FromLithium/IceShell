// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using System.Text;
using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Simple;
using IceShell.Parsing;
using IceShell.Parsing.Util;

/// <summary>
/// Defines a command that creates and displays command aliases.
/// </summary>
public static class AliasCommand
{
    private const string CommandName = "alias";
    private const string DeleteSwitch = "/d";

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
            PrintAll(shell.Dispatcher);
            return 0;
        }

        if (IsDeleteSwitch(args[0].Content))
        {
            if (args.Count < 2)
            {
                Console.Error.WriteLine(LangMessage.Get("shell.alias_delete_requires_name"));
                return 1;
            }

            return Delete(shell.Dispatcher, args[1].Content);
        }

        var name = args[0].Content;
        if (args.Count == 1)
        {
            if (!shell.Dispatcher.TryGetAlias(name, out var commandLine)
                || commandLine == null)
            {
                Console.Error.WriteLine(LangMessage.GetFormat("shell.alias_not_found", name));
                return 1;
            }

            Console.WriteLine($"{name}={commandLine}");
            return 0;
        }

        var builder = new StringBuilder();

        for (var i = 1; i < args.Count; i++)
        {
            if (i > 1)
            {
                builder.Append(' ');
            }

            builder.Append(FormatWord(args[i]));
        }

        shell.Dispatcher.SetAlias(name, builder.ToString());
        return 0;
    }

    private static int Delete(ISimpleCommandDispatcher dispatcher, string name)
    {
        if (!dispatcher.RemoveAlias(name))
        {
            Console.Error.WriteLine(LangMessage.GetFormat("shell.alias_not_found", name));
            return 1;
        }

        return 0;
    }

    private static void PrintAll(ISimpleCommandDispatcher dispatcher)
    {
        foreach (var pair in dispatcher.EnumerateAliasValues()
                     .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"{pair.Key}={pair.Value}");
        }
    }

    private static bool IsDeleteSwitch(string value)
    {
        return value.Equals(DeleteSwitch, StringComparison.OrdinalIgnoreCase)
               || value.Equals("-d", StringComparison.OrdinalIgnoreCase)
               || value.Equals("--delete", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatWord(SyntaxWord word)
    {
        if (word.QuoteType == WordQuoteType.Parentheses)
        {
            return FormatParentheses(word.Content);
        }

        if (word.QuoteType == WordQuoteType.None && CanRemainUnquoted(word.Content))
        {
            return word.Content;
        }

        return FormatQuoted(word.Content);
    }

    private static bool CanRemainUnquoted(string content)
    {
        return content.Length > 0
               && !content.Any(char.IsWhiteSpace)
               && !content.Contains('\\')
               && !content.Contains('"')
               && !content.StartsWith('(');
    }

    private static string FormatQuoted(string content)
    {
        var builder = new StringBuilder("\"");

        foreach (var ch in content)
        {
            if (ch == '\\' || ch == '"')
            {
                builder.Append('\\');
            }

            builder.Append(ch);
        }

        builder.Append('"');
        return builder.ToString();
    }

    private static string FormatParentheses(string content)
    {
        var builder = new StringBuilder("(");

        foreach (var ch in content)
        {
            if (ch == '\\' || ch == ')')
            {
                builder.Append('\\');
            }

            builder.Append(ch);
        }

        builder.Append(')');
        return builder.ToString();
    }
}
