// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("IceShell.Tests")]

namespace IceShell.Core;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Editor;
using IceShell.Core.CLI.Editor.Reboot;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using IceShell.Core.Settings;
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
        IRegistryCommandDispatcher? dispatcher = null,
        ILineEditor? lineEditor = null)
    {
        _settings = settings;
        _lineEditor = lineEditor ?? new RebootLineEditor();
        Prompt = DefaultPrompt;
        Dispatcher = dispatcher ?? new RegistryCommandDispatcher(settings.ExpandVariablesForAllValues);
        ModuleManager = new ModuleManager(Dispatcher);
    }

    /// <inheritdoc />
    public IModuleManager ModuleManager { get; }

    /// <inheritdoc />
    public string Prompt { get; set; }

    /// <inheritdoc />
    public IRegistryCommandDispatcher Dispatcher { get; }

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
        CommandResult cmdResult;

        try
        {
            var batchLine = Dispatcher.Parse(line, document);

            cmdResult = Dispatcher.Execute(this, document ?? _document, batchLine);
        }
        catch (CommandFormatException ex)
        {
            cmdResult = CommandResult.WithError(CommandErrorCode.GenericCommandFail, ex.Message);
        }
        catch (CommandInterruptException ex)
        {
            cmdResult = ex.Result;
        }
        catch (Win32Exception x) when (OperatingSystem.IsWindows() && x.NativeErrorCode == 740)
        {
            // 740 = Operating requires elevation
            ConsoleOutput.PrintShellError(string.Format(LangMessage.Get("shell_require_elevation"), LangMessage.Get("shell_windows_elevation_help")));
            cmdResult = CommandResult.WithError(CommandErrorCode.ElevationRequired);
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

    /// <inheritdoc />
    public CommandResult Execute(CommandSectionCompound compound, ICommandDocument? document = null)
    {
        return Dispatcher.Execute(this, document ?? _document, compound);
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
