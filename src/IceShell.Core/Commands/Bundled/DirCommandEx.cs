// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core;
using global::IceShell.Core.CLI.Languages;
using global::IceShell.Core.Commands;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Parsing;
using global::IceShell.Core.Api;
using NexusKrop.IceShell.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using global::IceShell.Core.Api.Native;

/// <summary>
/// Displays a list of files and subdirectories in a directory.
/// </summary>
/// <remarks>
/// <para>
/// When executed without redirection, this command prints a user-friendly table of directories and files
/// to the standard output.
/// </para>
/// <para>
/// When executed and instructed to redirect output to the next command, the command sends a list of directories
/// and files, with each line being a file or directory, and three null separated fields (of file name, size,
/// modification date). It looks like this (in example, two lines of strings in C#. In terminal it looks like lines of unseparated fields):
/// <code language="cs">
/// {
///     "text.txt\0123\2023-09-25T20:45:59.4034994",
///     "dir\0DIR\02023-09-25T20:45:59.4034994"
/// }
/// </code>
/// </para>
/// </remarks>
[CommandInfo("dir")]
internal class DirCommandEx : IShellCommand
{
    private sealed record class DirTableRow(string ShortDateTime, string Size, string FileName);

    private readonly List<DirTableRow> _rows = new();

    private int _fileCount;
    private int _dirCount;

    private int _dateLongest;
    private int _sizeLongest;
    private int _fileNameLongest;

    private string _dateFormat = "yyyy/MM/dd";
    private string _timeFormat = "HH:mm";
    private string _dir = Environment.CurrentDirectory;

    private string GetTableDateTime(DateTime time)
    {
        // Use a string builder
        // As we need to dynamically determine the space between sdStr and stStr
        var builder = new StringBuilder();

        // sdStr = short date string
        var sdStr = time.ToString(_dateFormat);

        // stStr = short time string
        var stStr = time.ToString(_timeFormat);

        // We add short date string first.
        builder.Append(sdStr);

        // This apply to Chinese culture
        // Not sure about others
        if (sdStr.Length == 9 && stStr.Length == 5)
        {
            // Use a single space
            builder.Append(' ');
        }
        else
        {
            // Use two spaces
            builder.Append("  ");
        }

        // Append the short time string
        builder.Append(stStr);

        // Return
        return builder.ToString();
    }

    private void PrintTable()
    {
        // Absolutely make sure you calibrated the table before printing.

        // Calculate the minimum character amounts required for columns.
        var dateMinimum = _dateLongest + 3;
        var sizeMinimum = _sizeLongest + 3;

        foreach (var row in _rows)
        {
            Console.Write(row.ShortDateTime);

            // If not enough characters to align the table, fill with white-spaces.
            if (row.ShortDateTime.Length < dateMinimum)
            {
                for (int i = row.ShortDateTime.Length; i < dateMinimum; i++)
                {
                    Console.Write(' ');
                }
            }

            // Do the same to DateTime string.
            Console.Write(row.Size);

            if (row.Size.Length < sizeMinimum)
            {
                for (int i = row.Size.Length; i < sizeMinimum; i++)
                {
                    Console.Write(' ');
                }
            }

            // File name is the last column, do not need to be aligned
            Console.WriteLine(row.FileName);
        }
    }

    // This method performs calibration by iterating through rows and compare them
    // until longest size and file name can be figured out.
    private void Calibrate()
    {
        foreach (var row in _rows)
        {
            if (_sizeLongest < row.Size.Length)
            {
                _sizeLongest = row.Size.Length;
            }

            if (_fileNameLongest < row.FileName.Length)
            {
                _fileNameLongest = row.FileName.Length;
            }
        }
    }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var realTarget = string.IsNullOrEmpty(TargetDir) ? Directory.GetCurrentDirectory() : PathSearcher.ExpandVariables(TargetDir);
        TextReader? pipeStream = null;

        // All chunked out ones need fix

        CommandChecks.DirectoryExists(realTarget);

        if (!string.IsNullOrWhiteSpace(realTarget))
        {
            _dir = realTarget;
        }

        if (!string.IsNullOrWhiteSpace(DateFormat))
        {
            _dateFormat = DateFormat;
        }

        if (!string.IsNullOrWhiteSpace(TimeFormat))
        {
            _timeFormat = TimeFormat;
        }

        // You cannot specify All Upper-case and All Lower-case at the same time.
        if (AllUpperCase && AllLowerCase)
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.Get("command.dir.error.cannot_have_all_upper_and_lower"));
        }

        if (Bare)
        {
            pipeStream = ExecuteBare();
        }
        else
        {
            if (context.NextAction == SyntaxNextAction.Redirect)
            {
                ExecuteStreamed(out pipeStream);
            }
            else
            {
                ExecuteDisplay();
            }
        }


        return CommandResult.Ok(pipeStream);
    }

    private TextReader ExecuteBare()
    {
        var ms = new MemoryStream();

        using (var writer = new StreamWriter(ms, leaveOpen: true))
        {
            var dirInfo = new DirectoryInfo(_dir);

            // Iterate through directories.
            foreach (var folder in dirInfo.GetDirectories())
            {
                if (!RevealHidden && IsHidden(folder))
                {
                    continue;
                }

                var procName = ProcessNameByOption(folder.Name);

                writer.WriteLine(procName);
            }

            // Do the same for files.
            foreach (var file in dirInfo.GetFiles())
            {
                if (!RevealHidden && IsHidden(file))
                {
                    continue;
                }

                var procName = ProcessNameByOption(file.Name);

                writer.WriteLine(procName);
            }
        }

        ms.Position = 0;
        return new StreamReader(ms);
    }

    private void ExecuteStreamed(out TextReader? pipeStream)
    {
        var sb = new StringBuilder();

        var dirInfo = new DirectoryInfo(_dir);

        // Iterate through directories.
        foreach (var folder in dirInfo.GetDirectories())
        {
            if (!RevealHidden && IsHidden(folder))
            {
                continue;
            }

            // Acquire info and last changed date/time for the entry.
            var modified = folder.LastWriteTime;

            var procName = ProcessNameByOption(folder.Name);

            // Add to row.
            sb.AppendFormat("{0}\0{1}\0{2}", procName, "DIR", modified.ToString("o")).AppendLine();
        }

        // Do the same for files.
        foreach (var file in dirInfo.GetFiles())
        {
            if (!RevealHidden && IsHidden(file))
            {
                continue;
            }

            var modified = file.LastWriteTime;

            var procName = ProcessNameByOption(file.Name);

            sb.AppendFormat("{0}\0{1}\0{2}", procName, file.Length.ToString(), modified.ToString("o")).AppendLine();
        }

        var str = sb.ToString();

        pipeStream = new StringReader(str);
    }

    // The code of printing human-friendly stuff are much more complex than
    // printing machine-readable stuff (mainly due to we have to align a table, nowadays these are covered
    // by Spectre Console anyway)
    private void ExecuteDisplay()
    {
        // Print DOS-like table title, sans the volume information (too complex).
        Console.WriteLine(LangMessage.Get("dir_directory_of"), _dir);
        Console.WriteLine();

        var dirInfo = new DirectoryInfo(_dir);

        // Iterate through directories.
        foreach (var folder in dirInfo.GetDirectories())
        {
            // Check if reveal hidden is off, and the entry is hidden. If both, conceal it.
            if (!RevealHidden && IsHidden(folder))
            {
                continue;
            }

            // Acquire info and last changed date/time for the entry.
            var modified = folder.LastWriteTime;

            // Generate text for date time.
            var tableDate = GetTableDateTime(modified);

            // We calibrate date first to save time later.
            if (_dateLongest < tableDate.Length)
            {
                _dateLongest = tableDate.Length;
            }

            // Add to row.
            _rows.Add(new(tableDate, "<DIR>", ProcessNameByOption(folder.Name)));

            // +1 to directory entry count.
            _dirCount++;
        }

        // Do the same for files.
        foreach (var file in dirInfo.GetFiles())
        {
            if (!RevealHidden && IsHidden(file))
            {
                continue;
            }

            var modified = file.LastWriteTime;

            var tableDate = GetTableDateTime(modified);

            if (_dateLongest < tableDate.Length)
            {
                _dateLongest = tableDate.Length;
            }

            _rows.Add(new(tableDate, file.Length.ToString(), ProcessNameByOption(file.Name)));
            _fileCount++;
        }

        Calibrate();
        PrintTable();

        // Print out the last line.
        // We do it manually here.
        for (int i = 0; i < _dateLongest; i++)
        {
            Console.Write(' ');
        }

        Console.WriteLine("{0} files, {1} directories", _fileCount, _dirCount);
    }

    private static bool IsHidden(FileSystemInfo info)
    {
        var windows = OperatingSystem.IsWindows();

        if (windows && (info.Attributes.HasFlag(FileAttributes.Hidden)
                    || info.Attributes.HasFlag(FileAttributes.System)))
        {
            return true;
        }

        if (!windows && info.Name.StartsWith('.'))
        {
            return true;
        }

        return false;
    }

    private string ProcessNameByOption(string name)
    {
        if (ShortName && OperatingSystem.IsWindows())
        {
            var shortName = WindowsFileApi.GetShortPathName(name);

            return AllLowerCase ? shortName.ToLower() : shortName;
        }

        if (AllUpperCase)
        {
            return name.ToUpper();
        }
        else if (AllLowerCase)
        {
            return name.ToLower();
        }

        return name;
    }

    [Value("directory", required: false, position: 0)]
    public string? TargetDir { get; set; }

    [Option('d', true)]
    public string? DateFormat { get; set; }

    [Option('t', true)]
    public string? TimeFormat { get; set; }

    [Option('h', false)]
    public bool RevealHidden { get; set; }

    // ***** Anniversary preview update options

    [Option('L', false)]
    public bool AllLowerCase { get; set; }

    [Option('U', false)]
    public bool AllUpperCase { get; set; }

    [Option('S', false)]
    public bool ShortName { get; set; }

    [Option('B', false)]
    public bool Bare { get; set; }
}
