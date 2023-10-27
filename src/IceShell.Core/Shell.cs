// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core;

using global::IceShell.Core;
using global::IceShell.Core.Api;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Exceptions;
using global::IceShell.Settings;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.CLI;
using NexusKrop.IceShell.Core.Completion;
using NexusKrop.IceShell.Core.Completion.Cache;
using NexusKrop.IceShell.Core.FileSystem;
using ReadLineReboot;
using System;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

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
    public Shell(ShellSettings settings)
    {
        _settings = settings;
        Prompt = DefaultPrompt;
        Dispatcher = new CommandDispatcher(this);
        ModuleManager = new ModuleManager(Dispatcher);
    }

    /// <inheritdoc />
    public IModuleManager ModuleManager { get; }

    /// <inheritdoc />
    public string Prompt { get; set; }

    /// <inheritdoc />
    public ICommandDispatcher Dispatcher { get; }

    /// <inheritdoc />
    public bool SupportsJump => false;

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
    /// Executes an executable located in <c>PATH</c>.
    /// </summary>
    /// <param name="fileName">The name of the executable. Subdirectories are prohibited.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <returns><see langword="true"/> if a valid executable was found; otherwise, <see langword="false"/>.</returns>
    public static int ExecuteOnPath(string fileName, IEnumerable<string>? args)
    {
        var actual = PathSearcher.SearchExecutable(fileName);

        if (actual == null)
        {
            return -255;
        }

        var startInfo = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = Environment.CurrentDirectory
        };

        args?.ForEach(startInfo.ArgumentList.Add);

        var proc = Process.Start(startInfo);

        if (proc == null)
        {
            return -500;
        }

        proc.WaitForExit();
        return proc.ExitCode;
    }

    /// <summary>
    /// Executes an executable located in <c>PATH</c>.
    /// </summary>
    /// <param name="fileName">The name of the executable. Subdirectories are prohibited.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <param name="reader">The reader to read the output.</param>
    /// <returns><see langword="true"/> if a valid executable was found; otherwise, <see langword="false"/>.</returns>
    public static int ExecuteOnPathRedirect(string fileName, IEnumerable<string>? args, out TextReader? reader)
    {
        var actual = PathSearcher.SearchExecutable(fileName);

        if (actual == null)
        {
            reader = null;
            return -255;
        }

        var startInfo = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = Environment.CurrentDirectory,
            RedirectStandardOutput = true
        };

        args?.ForEach(startInfo.ArgumentList.Add);

        var proc = Process.Start(startInfo);

        if (proc == null)
        {
            reader = null;
            return -500;
        }

        proc.WaitForExit();

        reader = proc.StandardOutput;
        return proc.ExitCode;
    }

    /// <summary>
    /// Parses and then executes the specified user input.
    /// </summary>
    /// <param name="line">The input.</param>
    /// <param name="actualExecutor">The executor to pass to commands (or have this instance to act on behalf of). If <see langword="null"/>, the Shell will run commands on its own behalf.</param>
    public CommandResult Execute(string line, ICommandExecutor? actualExecutor = null)
    {
        CommandResult cmdResult = CommandResult.WithError(CommandErrorCode.GenericCommandFail);

        try
        {
            var batchLine = Dispatcher.ParseLine(line);

            cmdResult = Dispatcher.Execute(batchLine, this);
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
    public CommandResult Execute(CommandSectionCompound compound, ICommandExecutor? actualExecutor = null)
    {
        return Dispatcher.Execute(compound, actualExecutor ?? this);
    }

    /// <summary>
    /// Starts the interactive shell process and logic on the current thread, and returns after the current
    /// shell instance have exited.
    /// </summary>
    /// <returns>The exit code.</returns>
    public int RunInteractive()
    {
        ReadLine.HistoryEnabled = true;
        ReadLine.AutoCompletionHandler = new ShellCompletionHandler(Dispatcher.CommandManager, Executive.DirCache);

        ModuleManager.LoadDirectory(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "modules"));
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

            var input = ReadLine.Read(prompt);

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var execResult = Execute(input, null);

            // Command failure handling
            if (execResult.ExitCode != 0 && execResult.ErrorCode != CommandErrorCode.None)
            {
                if (!string.IsNullOrWhiteSpace(execResult.Message))
                {
                    ConsoleOutput.PrintShellError(execResult.Message);
                }
                else
                {
                    ConsoleOutput.PrintShellError(LangMessage.MsgFromErrorCode(execResult.ErrorCode));
                }
            }

            Console.WriteLine();
        }

        return 0;
    }

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> as interactive shells should never support skipping through lines.
    /// </summary>
    /// <param name="label">The label to skip to. Ignored.</param>
    /// <exception cref="NotSupportedException">The action is not supported.</exception>
    public void Jump(string label)
    {
        throw new NotSupportedException();
    }
}
