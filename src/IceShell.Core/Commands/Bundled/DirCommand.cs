// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using System.Text;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Api.Native;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;
using IceShell.Parsing;

/// <summary>
///     Defines a command that displays a list of files and subdirectories in a directory. This class cannot be inherited.
/// </summary>
/// <remarks>
///     <para>
///         When executed without redirection, this command prints a user-friendly table of directories and files
///         to the standard output, unless <c>/B</c> was specified - in this case, displays a list of files and directories.
///     </para>
///     <para>
///         When executed and instructed to redirect output to the next command, the command sends a list of directories
///         and files, with each line being a file or directory, and three null separated fields (of file name, size,
///         modification date). It looks like this (in example, two lines of strings in C#. In terminal it looks like lines
///         of non-separated fields):
///         <code language="cs">
/// {
///     "text.txt\0123\2023-09-25T20:45:59.4034994",
///     "dir\0DIR\02023-09-25T20:45:59.4034994"
/// }
/// </code>
///     </para>
/// </remarks>
public sealed class DirCommand
{
    private const string ValueNameDirectory = "directory";
    private const char OptionDateFormat = 'd';
    private const char OptionTimeFormat = 't';
    private const char SwitchRevealHidden = 'h';
    private const char SwitchAllLowerCase = 'L';
    private const char SwitchAllUpperCase = 'U';
    private const char SwitchUseShortName = 'S';
    private const char SwitchBareMode = 'B';

    private const string MessageItemCount = "command.dir.item_count";

    private readonly List<DirTableRow> _rows = new();

    private string _dateFormat = "yyyy/MM/dd";

    private int _dateLongest;
    private string _dir = Environment.CurrentDirectory;
    private int _dirCount;

    private int _fileCount;
    private int _fileNameLongest;
    private int _sizeLongest;
    private string _timeFormat = "HH:mm";

    private DirCommand(CommandContext context)
    {
        TargetDirectory = context.GetValue(ValueNameDirectory)!;

        DateFormat = context.GetOptionValue(OptionDateFormat);
        TimeFormat = context.GetOptionValue(OptionTimeFormat);
        RevealHidden = context.GetSwitch(SwitchRevealHidden);
        AllLowerCase = context.GetSwitch(SwitchAllLowerCase);
        AllUpperCase = context.GetSwitch(SwitchAllUpperCase);
        UseShortName = context.GetSwitch(SwitchUseShortName);
        BareMode = context.GetSwitch(SwitchBareMode);
    }

    private string TargetDirectory { get; }

    private string? DateFormat { get; }

    private string? TimeFormat { get; }

    private bool RevealHidden { get; }

    // ***** Anniversary preview update options

    private bool AllLowerCase { get; }

    private bool AllUpperCase { get; }

    private bool UseShortName { get; }

    private bool BareMode { get; }

    /// <summary>
    ///     Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("dir", Routine)
            .ValueParameter(ValueNameDirectory, 0)
            .ValueOption(OptionDateFormat)
            .ValueOption(OptionTimeFormat)
            .SwitchOption(SwitchRevealHidden)
            .SwitchOption(SwitchAllLowerCase)
            .SwitchOption(SwitchAllUpperCase)
            .SwitchOption(SwitchUseShortName)
            .SwitchOption(SwitchBareMode)
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var instance = new DirCommand(context);
        return instance.Execute(context);
    }

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
                for (var i = row.ShortDateTime.Length; i < dateMinimum; i++)
                {
                    Console.Write(' ');
                }
            }

            // Do the same to DateTime string.
            Console.Write(row.Size);

            if (row.Size.Length < sizeMinimum)
            {
                for (var i = row.Size.Length; i < sizeMinimum; i++)
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

    private CommandResult Execute(CommandContext context)
    {
        var realTarget = string.IsNullOrEmpty(TargetDirectory)
            ? Directory.GetCurrentDirectory()
            : PathSearcher.ExpandVariables(TargetDirectory);
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
            return CommandResult.WithError(CommandErrorCode.BadArgument,
                LangMessage.Get("command.dir.error.cannot_have_all_upper_and_lower"));
        }

        if (BareMode)
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
            sb.Append($"{procName}\0DIR\0{modified:o}").AppendLine();
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

            sb.Append($"{procName}\0{file.Length.ToString()}\0{modified.ToString("o")}").AppendLine();
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
            _rows.Add(new DirTableRow(tableDate, "<DIR>", ProcessNameByOption(folder.Name)));

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

            _rows.Add(new DirTableRow(tableDate, file.Length.ToString(), ProcessNameByOption(file.Name)));
            _fileCount++;
        }

        Calibrate();
        PrintTable();

        // Print out the last line.
        // We do it manually here.
        for (var i = 0; i < _dateLongest; i++)
        {
            Console.Write(' ');
        }

        Console.WriteLine(LangMessage.Get(MessageItemCount), _fileCount, _dirCount);
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
        if (UseShortName && OperatingSystem.IsWindows())
        {
            var shortName = WindowsFileApi.GetShortPathName(name);

            return AllLowerCase ? shortName.ToLower() : shortName;
        }

        if (AllUpperCase)
        {
            return name.ToUpper();
        }

        if (AllLowerCase)
        {
            return name.ToLower();
        }

        return name;
    }

    private sealed record DirTableRow(string ShortDateTime, string Size, string FileName);
}