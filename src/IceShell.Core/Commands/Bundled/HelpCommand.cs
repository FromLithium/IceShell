// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using System.Text;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Utilities;
using Spectre.Console;

/// <summary>
/// Provides help information for IceShell commands.
/// </summary>
[CommandInfo("help")]
public static class HelpCommand
{
    private const string DescriptionMessageFormat = "command.{0}.description";
    private const string OptionMessageFormat = "command.{0}.option_{1}.description";
    private const string ValueMessageFormat = "command.{0}.value_{1}.description";
    private const string LiteralValues = "command.help.detailed.values";
    private const string LiteralSwitches = "command.help.detailed.switches";
    private const string LiteralOptions = "command.help.detailed.options";

    private const string ValueParameterCommandName = "command";

    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("help", Routine)
            .ValueParameter(ValueParameterCommandName, 0)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var commandName = context.GetValue(ValueParameterCommandName);

        return string.IsNullOrWhiteSpace(commandName) ? ExecuteSummary(shell) : ExecuteDetailed(shell, commandName);
    }

    private static CommandResult ExecuteDetailed(IShell shell, string commandName)
    {
        if (!DefaultCommands.TryGetCommandInfo(commandName, out var definition)
            || definition == null)
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.GetFormat("help_not_found", commandName ?? ""));
        }

        PrintHelp(definition);
        return CommandResult.Ok();
    }

    private static CommandResult ExecuteSummary(IShell shell)
    {
        var dispatcher = shell.Dispatcher;

        if (dispatcher.CommandCount == 0)
        {
            Console.WriteLine(LangMessage.Get("help_no_commands"));
            return CommandResult.Ok();
        }

        Console.WriteLine(LangMessage.Get("help_summary_more_information"));

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(" ", " ");

        var names = dispatcher.EnumerateCommandNames()
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

        foreach (var cmdName in names)
        {
            var description = string.Empty;

            if (DefaultCommands.TryGetCommandInfo(cmdName, out var item)
                && item != null)
            {
                description = LangMessage.Get(GetCommandDescription(item.Identifier));
            }

            grid.AddRow(Markup.Escape(cmdName), Markup.Escape(description));
        }

        AnsiConsole.Write(grid);

        return CommandResult.Ok();
    }

    private static string AssembleUsage(CommandRegistryEntry definition)
    {
        if (definition.CustomUsage != null)
        {
            return $" {definition.CustomUsage}";
        }

        var builder = new StringBuilder();
        builder.Append(' ');

        if (definition.Options.Any())
        {
            builder.Append("[options...]");
        }

        definition.Values.ForEach(x =>
        {
            if (x.Required)
            {
                builder.Append('<').Append(x.ReferenceName).Append('>');
            }
            else
            {
                builder.Append('[').Append(x.ReferenceName).Append(']');
            }

            builder.Append(' ');
        });

        return builder.ToString();
    }

    private static string GetCommandDescription(string commandId)
    {
        return string.Format(DescriptionMessageFormat, commandId);
    }

    internal static void PrintHelp(CommandRegistryEntry definition)
    {
        PrintHelp(definition, Array.Empty<string>());
    }

    internal static void PrintHelp(CommandRegistryEntry definition, IRegistryCommandDispatcher manager)
    {
        PrintHelp(definition, manager.EnumerateAliases(definition.Identifier));
    }

    internal static void PrintHelp(CommandRegistryEntry definition, IEnumerable<string> aliases)
    {
        Console.WriteLine(LangMessage.Get(string.Format(DescriptionMessageFormat, definition.Identifier)));
        Console.WriteLine();
        var usage = AssembleUsage(definition);

        foreach (var alias in aliases.Append(definition.Identifier)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("{0}{1}", alias, usage);
        }

        if (definition.Values.Any())
        {
            Console.WriteLine();

            // Values
            Console.WriteLine(LangMessage.Get(LiteralValues));
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            // Add a row for each command and their description.
            definition.Values.Select(x => x.ReferenceName)
                .ForEach(name =>
                    grid.AddRow(Markup.Escape(name),
                        // Now the description.
                        Markup.Escape(LangMessage.Get(string.Format(ValueMessageFormat,
                        // The ID.
                        definition.Identifier,
                        // The name.
                        name)))));

            AnsiConsole.Write(grid);
        }

        if (definition.Options.Any(x => !x.Value.IsSwitch))
        {
            Console.WriteLine();

            // Options
            Console.WriteLine(LangMessage.Get(LiteralOptions));
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (var option in 
                definition.Options.Where(x => !x.Value.IsSwitch))
            {
                grid.AddRow(Markup.Escape(!option.Value.IsSwitch ? $"/{option.Key}:<value>" : $"/{option.Key}"),
                    Markup.Escape(LangMessage.Get(string.Format(OptionMessageFormat, definition.Identifier, option.Key))));
            }

            AnsiConsole.Write(grid);
        }

        if (definition.Options.Any(x => x.Value.IsSwitch))
        {
            Console.WriteLine();

            // Switches
            Console.WriteLine(LangMessage.Get(LiteralSwitches));
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (var option in 
                definition.Options.Where(x => x.Value.IsSwitch))
            {
                grid.AddRow(Markup.Escape(!option.Value.IsSwitch ? $"/{option.Key}:<value>" : $"/{option.Key}"),
                    Markup.Escape(LangMessage.Get(string.Format(OptionMessageFormat, definition.Identifier, option.Key))));
            }

            AnsiConsole.Write(grid);
        }
    }
}
