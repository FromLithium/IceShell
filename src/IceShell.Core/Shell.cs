// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("IceShell.Tests")]

namespace IceShell.Core;

using System;
using System.Diagnostics;
using System.Reflection;
using IceShell.Core.Api;
using IceShell.Core.CLI.Editor;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Commands.Simple;
using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using IceShell.Core.Settings;
using IceShell.Core.Syntax;
using IceShell.Parsing;
using NexusKrop.IceShell.Core.CLI;

/// <summary>
/// Provides the shell experience of IceShell.
/// </summary>
public class Shell : IShell
{
    /// <summary>
    /// Gets the version of the shell interpreter application.
    /// </summary>
#if DEBUG
    public static readonly string AppVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion ?? "unknown";
#else
    public static readonly string AppVersion = FileVersionInfo.GetVersionInfo(Environment.ProcessPath ?? "iceshell.exe").ProductVersion ?? "unknown";
#endif

    private bool _exit;

    /// <summary>
    /// The default prompt string of the interactive shell.
    /// </summary>
    public const string DefaultPrompt = "$P$G ";

    private readonly ShellSettings _settings;
    private readonly ICommandDocument _document = new ShellCommandDocument();
    private readonly ILineEditor _lineEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Shell"/> class, with the default settings.
    /// </summary>
    public Shell() : this(new())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Shell"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="dispatcher">The dispatcher to use. If <see langword="null"/>, a default one is used.</param>
    /// <param name="lineEditor">The line editor implementation to use.</param>
    public Shell(ShellSettings settings,
        ISimpleCommandDispatcher? dispatcher = null,
        ILineEditor? lineEditor = null)
    {
        _settings = settings;
        _lineEditor = lineEditor ?? new BasicLineEditor();
        Prompt = DefaultPrompt;
        Dispatcher = dispatcher ?? new SimpleCommandDispatcher();

        // Module commands still use the legacy registry dispatcher and are not yet bridged into
        // the simple command dispatcher.
        ModuleManager = new ModuleManager(new RegistryCommandDispatcher());
    }

    /// <inheritdoc />
    public IModuleManager ModuleManager { get; }

    /// <inheritdoc />
    public string Prompt { get; set; }

    /// <inheritdoc />
    public ISimpleCommandDispatcher Dispatcher { get; }

    /// <inheritdoc/>
    public int LastExitCode { get; private set; }

    /// <inheritdoc/>
    public CommandErrorCode LastError { get; private set; }

    /// <summary>
    /// Quits this instance.
    /// </summary>
    public void Quit()
    {
        _exit = true;
    }

    /// <summary>
    /// Parses and then executes the specified user input.
    /// </summary>
    /// <param name="line">The input.</param>
    /// <param name="document">The executor to pass to commands (or have this instance to act on behalf of). If <see langword="null"/>, the Shell will run commands on its own behalf.</param>
    public CommandResult Execute(string line, ICommandDocument? document = null)
    {
        var actualDocument = document ?? _document;
        CommandResult cmdResult;

        try
        {
            var words = new CommandWordBreaker().RunParse(line);
            var resolvedWords = ExpandVariables(words, actualDocument);
            var exitCode = Dispatcher.Execute(this, actualDocument, resolvedWords);

            cmdResult = CommandResult.WithCode(exitCode);
        }
        catch (CommandFormatException ex)
        {
            cmdResult = CommandResult.WithError(CommandErrorCode.GenericCommandFail, ex.Message);
        }
        catch (FormatException ex)
        {
            cmdResult = CommandResult.WithError(CommandErrorCode.SyntaxError, ex.Message);
        }
        catch (Exception ex)
        {
            ConsoleOutput.PrintShellError(ex.ToString());
            cmdResult = CommandResult.WithException(CommandErrorCode.GenericCommandException, ex);
        }

        LastExitCode = cmdResult.ExitCode;
        LastError = cmdResult.ErrorCode;
        return cmdResult;
    }

    private IReadOnlyList<SyntaxWord> ExpandVariables(IReadOnlyList<SyntaxWord> words,
        ICommandDocument document)
    {
        if (words.Count == 0 || !_settings.ExpandVariablesForAllValues)
        {
            return words;
        }

        var result = new List<SyntaxWord>(words.Count)
        {
            ExpandWord(words[0], document)
        };

        DefaultCommands.TryGetCommandInfo(result[0].Content, out var info);

        for (var i = 1; i < words.Count; i++)
        {
            result.Add(ShouldExpandVariables(info, i - 1)
                ? ExpandWord(words[i], document)
                : words[i]);
        }

        return result;
    }

    private static SyntaxWord ExpandWord(SyntaxWord word, ICommandDocument document)
    {
        var expanded = PathSearcher.ExpandVariables(word.Content, document: document);

        return expanded == word.Content
            ? word
            : new SyntaxWord(expanded, word.QuoteType, word.StartsFrom);
    }

    private static bool ShouldExpandVariables(CommandRegistryEntry? info, int argumentIndex)
    {
        if (info == null)
        {
            return true;
        }

        if (info.NoParseVariables || info.VariableValues)
        {
            return false;
        }

        return argumentIndex >= info.Values.Count
               || !info.Values[argumentIndex].NoExpandVariables;
    }

    /// <summary>
    /// Starts the interactive shell process and logic on the current thread, and returns after the current
    /// shell instance have exited.
    /// </summary>
    /// <returns>The exit code.</returns>
    public int RunInteractive()
    {
        _lineEditor.Initialize(Dispatcher);
        ModuleManager.Initialize();

        // show date and time if allowed
        if (_settings.DisplayDateTimeOnStartUp)
        {
            var time = DateTime.Now;

            Console.WriteLine(LangMessage.Get("shell_current_date"), time.ToShortDateString());
            Console.WriteLine(LangMessage.Get("shell_current_time"), time.ToShortTimeString());
        }

        if (_settings.DisplayShellInfoOnStartUp)
        {
            Console.WriteLine(LangMessage.Get("ver_line_0"), AppVersion);
        }

        // Add an empty line afterwards
        Console.WriteLine();

        while (!_exit && !Executive.Interrupt)
        {
            _ = Executive.TryParsePrompt(this.Prompt, out var prompt);

            prompt ??= "[prompt error]> ";

            var input = _lineEditor.DoReadLine(Dispatcher, prompt);

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var execResult = Execute(input);

            // Command failure handling
            if (execResult.ExitCode != 0 && execResult.ErrorCode != CommandErrorCode.None)
            {
                ConsoleOutput.PrintShellError(!string.IsNullOrWhiteSpace(execResult.Message)
                    ? execResult.Message
                    : LangMessage.MsgFromErrorCode(execResult.ErrorCode));
            }

            Console.WriteLine();
        }

        return 0;
    }
}
