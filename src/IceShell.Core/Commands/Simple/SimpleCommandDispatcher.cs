// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Simple;

using System.ComponentModel;
using System.Diagnostics;
using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;
using IceShell.Core.Syntax;
using IceShell.Core.Utilities;
using IceShell.Parsing;
using IceShell.Parsing.Util;
using NexusKrop.IceShell.Core.FileSystem;

/// <summary>
/// Provides the default implementation of the <see cref="ISimpleCommandDispatcher"/> interface.
/// </summary>
public sealed class SimpleCommandDispatcher : ISimpleCommandDispatcher
{
    private const int ExitCodeNotExecutable = 126;
    private const int ExitCodeCommandNotFound = 127;
    private const int ExitCodeUnexpectedError = 255;

    private readonly Dictionary<string, InternalCommand> _commands =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, CommandRegistryEntry> _commandInfo =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _aliases = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleCommandDispatcher"/> class.
    /// </summary>
    public SimpleCommandDispatcher() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleCommandDispatcher"/> class with the
    /// specified options.
    /// </summary>
    /// <param name="options">The options to use, or <see langword="null"/> to use the defaults.</param>
    public SimpleCommandDispatcher(SimpleCommandDispatcherOptions? options)
    {
        Options = options ?? new SimpleCommandDispatcherOptions();
    }

    /// <summary>
    /// Gets the options of this dispatcher.
    /// </summary>
    public SimpleCommandDispatcherOptions Options { get; }

    /// <inheritdoc />
    public int AliasCount => _aliases.Count;

    /// <inheritdoc />
    public int CommandCount => _commands.Count;

    /// <inheritdoc />
    public void Register(string name, InternalCommand command)
    {
        Register(name, command, null);
    }

    /// <inheritdoc />
    public void Register(string name, InternalCommand command, CommandRegistryEntry? info)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(command);

        if (!_commands.TryAdd(name, command))
        {
            throw new ArgumentException("A command with the same name already exists.", nameof(name));
        }

        if (info != null)
        {
            _commandInfo[name] = info;
        }
    }

    /// <inheritdoc />
    public bool HasCommand(string name)
    {
        return _commands.ContainsKey(name);
    }

    /// <inheritdoc />
    public bool TryGetCommand(string name, out InternalCommand? command)
    {
        return _commands.TryGetValue(name, out command);
    }

    /// <inheritdoc />
    public bool TryGetCommandInfo(string name, out CommandRegistryEntry? info)
    {
        return _commandInfo.TryGetValue(name, out info);
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateCommandNames()
    {
        return _commands.Keys;
    }

    /// <inheritdoc />
    public void SetAlias(string alias, string commandLine)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        ArgumentNullException.ThrowIfNull(commandLine);

        _aliases[alias] = commandLine;
    }

    /// <inheritdoc />
    public bool RemoveAlias(string alias)
    {
        return _aliases.Remove(alias);
    }

    /// <inheritdoc />
    public bool HasAlias(string alias)
    {
        return _aliases.ContainsKey(alias);
    }

    /// <inheritdoc />
    public bool TryGetAlias(string alias, out string? commandLine)
    {
        return _aliases.TryGetValue(alias, out commandLine);
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases()
    {
        return _aliases.Keys;
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateAliases(string commandName)
    {
        return _aliases.Where(x => GetFirstWord(x.Value)
                .Equals(commandName, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Key);
    }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string>> EnumerateAliasValues()
    {
        return _aliases;
    }

    /// <inheritdoc />
    public int Execute(IShell shell, ICommandDocument document, IReadOnlyList<SyntaxWord> words)
    {
        return ExecuteCore(shell, document, words, 0);
    }

    private int ExecuteCore(IShell shell,
        ICommandDocument document,
        IReadOnlyList<SyntaxWord> words,
        int aliasDepth)
    {
        if (words.Count == 0)
        {
            return 0;
        }

        var nameWord = words[0];
        var name = nameWord.Content;

        if (string.IsNullOrWhiteSpace(name))
        {
            return 0;
        }

        var externalOnly = IsExternalOnly(nameWord);

        if (!externalOnly && _aliases.TryGetValue(name, out var aliasLine))
        {
            return ExecuteAlias(shell, document, aliasLine, words, aliasDepth + 1);
        }

        var preferExternal = externalOnly
                             || (Options.ResolveNonFilePathToExternalCommand && !IsFilePath(name));

        if (!preferExternal && _commands.TryGetValue(name, out var command))
        {
            return ExecuteInternal(shell, document, command, words);
        }

        return ExecuteExternal(name, words);
    }

    private int ExecuteAlias(IShell shell,
        ICommandDocument document,
        string aliasLine,
        IReadOnlyList<SyntaxWord> invocation,
        int aliasDepth)
    {
        if (aliasDepth > Options.MaxAliasDepth)
        {
            Console.Error.WriteLine(LangMessage.GetFormat("shell.alias_too_deep", Options.MaxAliasDepth));
            return ExitCodeUnexpectedError;
        }

        var expanded = new List<SyntaxWord>();

        try
        {
            expanded.AddRange(new CommandWordBreaker().RunParse(aliasLine));
        }
        catch (FormatException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitCodeUnexpectedError;
        }

        for (var i = 1; i < invocation.Count; i++)
        {
            expanded.Add(invocation[i]);
        }

        return ExecuteCore(shell, document, expanded, aliasDepth);
    }

    private static int ExecuteInternal(IShell shell,
        ICommandDocument document,
        InternalCommand command,
        IReadOnlyList<SyntaxWord> words)
    {
        var args = new SyntaxWord[words.Count - 1];
        for (var i = 1; i < words.Count; i++)
        {
            args[i - 1] = words[i];
        }

        try
        {
            return command(shell, document, args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return ExitCodeUnexpectedError;
        }
    }

    private int ExecuteExternal(string name, IReadOnlyList<SyntaxWord> words)
    {
        var path = ResolveExternalPath(name);

        if (path == null || !File.Exists(path))
        {
            Console.Error.WriteLine(LangMessage.GetFormat("shell.unknown_command", name));
            return ExitCodeCommandNotFound;
        }

        if (!FileUtility.IsExecutable(path))
        {
            if (!Options.UseShellExecuteForNonExecutableFiles)
            {
                Console.Error.WriteLine(LangMessage.MsgInvalidExecutable(name));
                return ExitCodeNotExecutable;
            }

            return ShellExecute(path, words);
        }

        var startInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = false,
            WorkingDirectory = Environment.CurrentDirectory
        };

        for (var i = 1; i < words.Count; i++)
        {
            startInfo.ArgumentList.Add(words[i].Content);
        }

        try
        {
            var process = Process.Start(startInfo);

            if (process == null)
            {
                Console.Error.WriteLine(LangMessage.MsgUnableStartProcess());
                return ExitCodeUnexpectedError;
            }

            Executive.WatchProcess(process);
            process.WaitForExit();

            return process.ExitCode;
        }
        catch (Win32Exception ex) when (OperatingSystem.IsWindows() && ex.NativeErrorCode == 740)
        {
            Console.Error.WriteLine(string.Format(LangMessage.Get("shell_require_elevation"),
                LangMessage.Get("shell_windows_elevation_help")));
            return ExitCodeNotExecutable;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return ExitCodeUnexpectedError;
        }
    }

    private static int ShellExecute(string path, IReadOnlyList<SyntaxWord> words)
    {
        var startInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = true
        };

        for (var i = 1; i < words.Count; i++)
        {
            startInfo.ArgumentList.Add(words[i].Content);
        }

        try
        {
            var process = Process.Start(startInfo);

            if (process == null)
            {
                Console.Error.WriteLine(LangMessage.MsgUnableStartProcess());
                return ExitCodeUnexpectedError;
            }

            Executive.WatchProcess(process);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return ExitCodeUnexpectedError;
        }
    }

    private static string? ResolveExternalPath(string name)
    {
        try
        {
            return IsFilePath(name)
                ? Path.GetFullPath(name, Environment.CurrentDirectory)
                : ExecutableSearcher.FindSystem(name);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool IsExternalOnly(SyntaxWord word)
    {
        return word.QuoteType != WordQuoteType.None
               || IsFilePath(word.Content);
    }

    private static bool IsFilePath(string name)
    {
        return name.StartsWith('.')
               || Path.IsPathFullyQualified(name)
               || name.Contains(Path.DirectorySeparatorChar)
               || name.Contains(Path.AltDirectorySeparatorChar);
    }

    private static string GetFirstWord(string commandLine)
    {
        try
        {
            var words = new CommandWordBreaker().RunParse(commandLine);
            return words.Count == 0 ? string.Empty : words[0].Content;
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }
}
