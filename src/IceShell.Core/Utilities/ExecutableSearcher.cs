// SPDX-FileCopyrightText: 2023, 2026 WithLithum
// SPDX-License-Identifier: GPL-3.0-or-later

namespace IceShell.Core.Utilities;

/// <summary>
/// Provides helpers for locating commands in <c>PATH</c>.
/// </summary>
public static class ExecutableSearcher
{
    /// <summary>
    /// Searches for an external command matching the specified file path. If available,
    /// <c>PATHEXT</c> extensions will also be tested.
    /// </summary>
    /// <param name="fileName">The file name to test.</param>
    /// <returns>
    /// A probable path to the command specified by <paramref name="fileName"/>. The file is
    /// <i>not</i> guaranteed to exist.
    /// </returns>
    public static string? Find(string fileName)
    {
        var pathExtString = Environment.GetEnvironmentVariable("PATHEXT");
        if (pathExtString == null)
        {
            return null;
        }

        return TestPathForExtCore(null, fileName, pathExtString);
    }

    /// <summary>
    /// Searches the specified directory for an  external command matching the specified command
    /// name. If available, <c>PATHEXT</c> extensions will also be tested.
    /// </summary>
    /// <param name="fileName">The file name to test.</param>
    /// <param name="directory">The base directory to test.</param>
    /// <returns>
    /// A probable path to the command specified by <paramref name="fileName"/>. The file is
    /// <i>not</i> guaranteed to exist.
    /// </returns>
    public static string? Find(string fileName, string directory)
    {
        var pathExtString = Environment.GetEnvironmentVariable("PATHEXT");
        if (pathExtString == null)
        {
            return null;
        }

        return TestPathForExtCore(directory, fileName, pathExtString);
    }

    /// <summary>
    /// Searches the current working directory and the <c>PATH</c> environment variable for an
    /// external command matching the specified command name.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="path">
    /// A list of directory paths to search the command name in, separated by
    /// <see cref="Path.PathSeparator"/>. If <see langword="null"/>, defaults to the value of
    /// <c>PATH</c> environment variable.
    /// </param>
    /// <param name="pathExtensions">
    /// The path extensions to use. Defaults to the value of <c>PATHEXT</c> environment variable.
    /// </param>
    /// <param name="currentDirectory">
    /// The directory to begin searching from.
    /// </param>
    /// <param name="matchCurrentDirectory">
    /// If <see langword="true"/>, search in the current directory before searching in directories
    /// in <paramref name="path"/>.
    /// </param>
    /// <returns>
    /// A probable path to the command specified by <paramref name="commandName"/>. The file is
    /// <i>not</i> guaranteed to exist.
    /// </returns>
    public static string? FindSystem(string commandName,
        string? path = null,
        string? pathExtensions = null,
        string? currentDirectory = null,
        bool matchCurrentDirectory = false)
    {
        // If absolute path, return the absolute path.
        if (Path.IsPathFullyQualified(commandName))
        {
            return commandName;
        }

        currentDirectory ??= Environment.CurrentDirectory;

        // If relative path, resolve full path and return.
        if (commandName.Contains(Path.DirectorySeparatorChar))
        {
            return Path.GetFullPath(commandName,
                currentDirectory);
        }

        // If pathext exists, search every extension specified
        pathExtensions ??= Environment.GetEnvironmentVariable("PATHEXT");
        if (commandName.Contains('.') && pathExtensions != null)
        {
            string? pathDest = (matchCurrentDirectory
                ? TestPathForExtCore(currentDirectory,
                    commandName,
                    pathExtensions)
                : null)
                ?? SearchPathExtCore(commandName, pathExtensions, path);
            if (pathDest != null)
            {
                return pathDest;
            }
        }

        // Finally attempt to search for PATH for the file name as is.
        string? searchPathResult = (matchCurrentDirectory
            ? SearchDirectoryCore(commandName, currentDirectory)
            : null)
            ?? SearchPathCore(commandName, path);
        if (searchPathResult != null)
        {
            return searchPathResult;
        }

        return null;
    }

    private static string? SearchDirectoryCore(string commandName,
        string directory)
    {
        string path = Path.GetFullPath(commandName, directory);
        if (File.Exists(path))
        {
            return path;
        }

        return null;
    }

    private static string? SearchPathCore(string commandName,
        string? pathString = null)
    {
        pathString ??= Environment.GetEnvironmentVariable("PATH");
        if (pathString == null)
        {
            return null;
        }

        // TODO move the split above so we don't allocate too many times.
        foreach (var path in pathString.Split(Path.PathSeparator))
        {
            string? testedPath = Path.GetFullPath(commandName, path);
            if (File.Exists(testedPath))
            {
                return testedPath;
            }
        }

        return null;
    }

    private static string? SearchPathExtCore(string commandName,
        in ReadOnlySpan<char> pathExtBuf,
        string? pathString = null)
    {
        pathString ??= Environment.GetEnvironmentVariable("PATH");
        if (pathString == null)
        {
            return null;
        }

        foreach (var path in pathString.Split(Path.PathSeparator))
        {
            string? testedPath = TestPathForExtCore(path, commandName, in pathExtBuf);
            if (testedPath != null)
            {
                return testedPath;
            }
        }

        return null;
    }

    private static string? TestPathForExtCore(string? baseDirectory,
        string commandName,
        in ReadOnlySpan<char> pathExt)
    {
        foreach (Range range in pathExt.Split(Path.PathSeparator))
        {
            string testPath = $"{commandName}{pathExt[range]}";
            if (baseDirectory != null)
            {
                testPath = Path.GetFullPath(testPath, baseDirectory);
            }

            if (File.Exists(testPath))
            {
                return testPath;
            }
        }

        // Perform exact path test
        string exactPath = Path.GetFullPath(commandName,
            baseDirectory ?? Environment.CurrentDirectory);
        if (File.Exists(exactPath))
        {
            return exactPath;
        }

        return null;
    }
}
