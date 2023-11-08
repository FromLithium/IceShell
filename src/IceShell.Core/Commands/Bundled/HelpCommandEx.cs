// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using NexusKrop.IceCube.Util.Enumerables;
using Spectre.Console;
using System.Text;

/// <summary>
/// Provides help information for IceShell commands.
/// </summary>
[CommandInfo("help")]
public class HelpCommandEx : IShellCommand
{
    private const string DescriptionMessageFormat = "command.{0}.description";
    private const string OptionMessageFormat = "command.{0}.option_{1}.description";
    private const string ValueMessageFormat = "command.{0}.value_{1}.description";

    /// <summary>
    /// Gets or sets the name of the command that the user wants help information.
    /// </summary>
    /// <value>
    /// The name of the command that the user wants help information. If <see langword="null"/>, prints a list of commands.
    /// </value>
    [Value("command", false, position: 0)]
    public string? CommandName { get; set; }

    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var commandName = CommandName;

        if (string.IsNullOrWhiteSpace(commandName))
        {
            return ExecuteSummary(shell);
        }

        return ExecuteDetailed(shell, commandName);
    }

    private static CommandResult ExecuteDetailed(IShell shell, string commandName)
    {
        var commandType = shell.Dispatcher.CommandManager.GetDefinition(commandName);

        if (commandType == null)
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.GetFormat("help_not_found", commandName ?? ""));
        }

        var def = commandType.Definition;

        PrintHelp(def, shell.Dispatcher.CommandManager);
        return CommandResult.Ok();
    }

    private static CommandResult ExecuteSummary(IShell shell)
    {
        if (!shell.Dispatcher.CommandManager.Any())
        {
            Console.WriteLine(LangMessage.Get("help_no_commands"));
            return CommandResult.Ok();
        }

        Console.WriteLine(LangMessage.Get("help_summary_more_information"));

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(" ", " ");

        var keysEnumerable = shell.Dispatcher.CommandManager.CommandAliases;
        var keys = new List<string>(keysEnumerable);
        keys.Sort();

        foreach (var cmdName in keys)
        {
            var item = shell.Dispatcher.CommandManager.GetDefinition(cmdName);

            if (item == null)
            {
                continue;
            }

            grid.AddRow(Markup.Escape(cmdName), Markup.Escape(LangMessage.Get(GetCommandDescription(item.Id))));
        }

        AnsiConsole.Write(grid);

        return CommandResult.Ok();
    }

    private static string AssembleUsage(CommandDefinition definition)
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
                builder.Append('<').Append(x.Name).Append('>');
            }
            else
            {
                builder.Append('[').Append(x.Name).Append(']');
            }

            builder.Append(' ');
        });

        return builder.ToString();
    }

    private static string GetCommandDescription(string commandId)
    {
        return string.Format(DescriptionMessageFormat, commandId);
    }

    private static void PrintHelp(CommandDefinition definition, ICommandManager manager)
    {
        Console.WriteLine(LangMessage.Get(string.Format(DescriptionMessageFormat, definition.Id)));
        Console.WriteLine();
        var usage = AssembleUsage(definition);

        foreach (var alias in manager.GetBoundAliases(definition.Id))
        {
            Console.WriteLine("{0}{1}", alias, usage);
        }

        if (definition.Values.Any())
        {
            Console.WriteLine();

            // Values
            Console.WriteLine("Values: ");
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            // Add a row for each command and their description.
            definition.Values.Select(x => x.Name)
                .ForEach(name =>
                    grid.AddRow(Markup.Escape(name),
                        // Now the description.
                        Markup.Escape(LangMessage.Get(string.Format(ValueMessageFormat,
                        // The ID.
                        definition.Id,
                        // The name.
                        name)))));

            AnsiConsole.Write(grid);
        }

        if (definition.Options.Any())
        {
            Console.WriteLine();

            // Options
            Console.WriteLine("Options: ");
            Console.WriteLine();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (var option in definition.Options)
            {
                grid.AddRow(Markup.Escape(option.Value.HasValue ? $"/{option.Key}" : $"/{option.Key}:<value>"),
                    Markup.Escape(LangMessage.Get(string.Format(OptionMessageFormat, definition.Id, option.Key))));
            }

            AnsiConsole.Write(grid);
        }
    }
}
