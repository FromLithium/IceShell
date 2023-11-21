// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core;

using IceShell.Core.Commands;
using IceShell.Settings;
using NexusKrop.IceCube;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.Completion.Cache;
using NexusKrop.IceShell.Core.FileSystem;
using ReadLineReboot;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using IceShell.Core.FileSystem;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

/// <summary>
/// Provides executive services for the IceShell processes.
/// </summary>
public static class Executive
{
    private static ShellSettings? InstanceSettings;
    internal static readonly DirCache DirCache = new(Environment.CurrentDirectory);

    #region Start Info handling
    internal static ProcessStartInfo CreateStartInfo(CommandSection section)
    {
        return CreateStartInfo(section.Name,
            !(section.Name.StartsWith('.') || Path.IsPathFullyQualified(section.Name)),
            section.Statements?.Select(x => x.Content) ?? Enumerable.Empty<string>());
    }
    
    private static ProcessStartInfo CreateStartInfo(string relativeFile, bool searchInPath, IEnumerable<string>? args)
    {
        var actual = searchInPath ? PathSearcher.SearchExecutable(relativeFile)
            : Path.GetFullPath(relativeFile, Directory.GetCurrentDirectory());

        if (actual == null || !File.Exists(actual))
        {
            throw new FileNotFoundException("Cannot find file", relativeFile);
        }

        var result = new ProcessStartInfo(actual)
        {
            UseShellExecute = false
        };

        args?.ForEach(result.ArgumentList.Add);

        return result;
    }

    internal static bool TryCreateStartInfo(CommandSection section, out ProcessStartInfo? result, out bool isExecutable)
    {
        return TryCreateStartInfo(section.Name,
            !(section.Name.StartsWith('.') || Path.IsPathFullyQualified(section.Name)),
            section.Statements?.Select(x => x.Content) ?? Enumerable.Empty<string>(),
            out result,
            out isExecutable);
    }

    private static bool TryCreateStartInfo(string relativeFile, bool searchInPath, IEnumerable<string>? args,
        out ProcessStartInfo? result, out bool isExecutable)
    {
        var actual = searchInPath ? PathSearcher.SearchExecutable(relativeFile)
            : Path.GetFullPath(relativeFile, Directory.GetCurrentDirectory());

        if (actual == null || !File.Exists(actual))
        {
            result = null;
            isExecutable = false;
            return false;
        }

        var executable = FileUtility.IsExecutable(actual);
        isExecutable = executable;

        result = new ProcessStartInfo(actual)
        {
            UseShellExecute = !executable
        };

        args?.ForEach(result.ArgumentList.Add);

        return true;
    }
    #endregion

    private static Process? WatchingProcess { get; set; }

    internal static bool Interrupt { get; private set; }

    /// <summary>
    /// Searches for files from the current directory based on the specified pattern. 
    /// </summary>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>The search result.</returns>
    public static IEnumerable<string> GlobFiles(string pattern)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var matcher = new Matcher();

        matcher.AddInclude(PathSearcher.ExpandVariables(pattern));

        var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(currentDir)));

        return result.Files.Select(x => Path.Combine(currentDir, x.Path));
    }
    
    internal static void WatchProcess(Process process)
    {
        if (process == null)
        {
            Debug.WriteLine("Warning: WatchProcess will no longer accept null processes starting from v0.3.1.");
            return;
        }

        WatchingProcess = process;
    }

    /// <summary>
    /// Attaches the executive services to the current process.
    /// </summary>
    public static void Attach(ShellSettings settings)
    {
        LoadHistory();

        Console.CancelKeyPress += Console_CancelKeyPress;

        if (OperatingSystem.IsLinux())
        {
            PosixSignalRegistration.Create(PosixSignal.SIGTERM, OnTermination);
        }

        InstanceSettings = settings;
    }

    private static void OnTermination(PosixSignalContext context)
    {
        if (WatchingProcess?.HasExited == false)
        {
            WatchingProcess.Kill();
        }

        Interrupt = true;
        Exit();
    }

    private static void LoadHistory()
    {
        try
        {
            var pathToHistory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".iceShell", "history.txt");

            if (File.Exists(pathToHistory))
            {
                var list = new List<string>();

                list.AddRange(File.ReadLines(pathToHistory));

                ReadLine.SetHistory(list);
            }

            ReadLine.HistorySize = 100;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to load history!");
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Stores all history and exits the process.
    /// </summary>
    /// <param name="exitCode"></param>
    public static void Exit(int exitCode = 0)
    {
        SaveHistory();

        Environment.Exit(exitCode);
    }

    private static void SaveHistory()
    {
        try
        {
            var stream = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".iceShell", "history.txt"));

            using var writer = new StreamWriter(stream);
            ReadLine.GetHistory().ForEach(writer.WriteLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to save history!");
            Console.WriteLine(ex);
        }
    }

    private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (WatchingProcess == null || WatchingProcess.HasExited)
        {
            // This is only a provisional solution because we cannot break out of ReadLine.

            Exit();
            return;
        }

        e.Cancel = true;

        if ((InstanceSettings?.TerminateUponInterrupt == true) && OperatingSystem.IsLinux())
        {
            WatchingProcess.EndGracefully();
        }
        else
        {
            WatchingProcess.Kill();
        }

        WatchingProcess = null;
    }

    /// <summary>
    /// Parses a standard prompt template.
    /// </summary>
    /// <param name="template">The prompt template.</param>
    /// <param name="result">The parsing result.</param>
    /// <returns><see langword="true" /> if the parsing is successful; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <para>
    /// The prompt placeholders are listed below.
    /// </para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Value</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><c>$Q</c></term>
    ///         <description>The equal sign (<c>=</c>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$$</c></term>
    ///         <description>The dollar sign (<c>$</c>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$T</c></term>
    ///         <description>The current time in short time format</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$D</c></term>
    ///         <description>The current date in short date format</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$P</c></term>
    ///         <description>The full path of current directory</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$V</c></term>
    ///         <description>The IceShell version (<see cref="Shell.AppVersion"/>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$G</c></term>
    ///         <description>The greater than sign (<c><![CDATA[>]]></c>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$L</c></term>
    ///         <description>The lesser than sign (<c><![CDATA[<]]></c>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$B</c></term>
    ///         <description>The pipe sign (<c>|</c>)</description>
    ///     </item>
    ///     <item>
    ///         <term><c>$_</c></term>
    ///         <description>System new-line sequence (<see cref="Environment.NewLine"/>)</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static bool TryParsePrompt(ReadOnlySpan<char> template, out string? result)
    {
        var insidePlaceholder = false;

        var now = DateTime.Now;
        var builder = new StringBuilder();

        var answers = new Dictionary<char, string>()
        {
            { 'Q', "=" },
            { '$', "$" },
            { 'T', now.ToShortTimeString() },
            { 'D', now.ToShortDateString() },
            { 'P', Environment.CurrentDirectory },
            { 'V', Shell.AppVersion },
            { 'G', ">" },
            { 'L', "<" },
            { 'B', "|" },
            { '_', Environment.NewLine }
        };

        foreach (var ch in template)
        {
            if (!insidePlaceholder)
            {
                // If not in variable
                if (ch == '$')
                {
                    insidePlaceholder = true;
                    continue;
                }

                builder.Append(ch);
            }
            else
            {
                // If already in variable
                insidePlaceholder = false;

                if (!answers.TryGetValue(ch, out var answer))
                {
                    // Failure - Invalid placeholder

                    result = null;
                    return false;
                }

                builder.Append(answer);
            }
        }

        if (insidePlaceholder)
        {
            // Failure - Placeholder never ends

            result = null;
            return false;
        }

        result = builder.ToString();
        return true;
    }


    /// <summary>
    /// Changes the current directory of this process.
    /// </summary>
    /// <param name="target">The target directory. Must be a system path.</param>
    public static void ChangeDirectory(string target)
    {
        DirCache.UpdateDirectory(target);
        Directory.SetCurrentDirectory(target);
    }

}
