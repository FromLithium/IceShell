// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.FileSystem;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Exceptions;
using JetBrains.Annotations;
using NexusKrop.IceShell.Core.FileSystem;
using Spectre.Console;

/// <summary>
/// Provides utilities for a custom path system.
/// </summary>
public static class PathSearcher
{
    private const string ComFileExtensionFormat = "{0}.com";
    private const string ExeFileExtensionFormat = "{0}.exe";

    private static readonly IEnumerable<string> Paths;

#pragma warning disable S3963
    // This is a big chunk of complex code that SonarLint is too dumb to detect.
    static PathSearcher()
#pragma warning restore S3963
    {
        var result = new HashSet<string>();

        var path = Environment.GetEnvironmentVariable("PATH");

        if (path == null)
        {
            // Doing so will cause the shell cannot find anything in PATH because
            // there was no PATH.

            AnsiConsole.MarkupLineInterpolated(FormattableStringFactory.Create("<red>{0}</red>", LangMessage.Get("shell_no_path")));
            Console.WriteLine();

            // Set PATHS to an empty string enumerable.

            Paths = Enumerable.Empty<string>();
        }
        else
        {
            foreach (var p in path.Split(Path.PathSeparator))
            {
                result.Add(p);
            }
        }

        Paths = result.ToImmutableHashSet();
    }

    /// <summary>
    /// Ensures that the specified path is valid.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <exception cref="FormatException">The path is invalid.</exception>
    public static void CheckPath(string path)
    {
        var invalidPathChars = Path.GetInvalidPathChars();

        if (invalidPathChars.Any(path.Contains))
        {
            throw ExceptionHelper.InvalidPath();
        }
    }

    /// <summary>
    /// Ensures that the specified path is valid.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <exception cref="FormatException">The path is invalid.</exception>
    [PublicAPI]
    public static void CheckPath(ReadOnlySpan<char> path)
    {
        if (!IsPathValid(path))
        {
            throw ExceptionHelper.InvalidPath();
        }
    }

    /// <summary>
    /// Determines that whether the specified path contains any of the invalid path characters as
    /// set out by the operating system.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns><see langword="true"/> if the path is valid; otherwise, <see langword="false"/>.</returns>
    [PublicAPI]
    public static bool IsPathValid(ReadOnlySpan<char> path)
    {
        var invalidPathChars = Path.GetInvalidPathChars();

        foreach (var ch in path)
        {
            if (invalidPathChars.Contains(ch))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Expands the home directory section of the specified path.
    /// </summary>
    /// <param name="path">The path to expand.</param>
    /// <returns>The expanded path.</returns>
    public static string ExpandHome(ReadOnlySpan<char> path)
    {
        // If it is solely "~" symbol then do not bother do the rest of the processing and
        // return the HOME.
        if (path.Length == 1 && path[0] == '~')
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        if (path[0] != '~')
        {
            return new string(path);
        }

        var livePath = path[1..];
        if (livePath[0] == Path.DirectorySeparatorChar)
        {
            livePath = path[2..];
        }
        
        var nextPath = new string(livePath);
        
        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var result = Path.Combine(profile, nextPath);
        return result;
    }
    
    /// <summary>
    /// Evaluate a path and expands all environment variables within.
    /// </summary>
    /// <param name="path">The path to expand.</param>
    /// <param name="lenient">If <see langword="true"/>, unknown or malformed environment variables will not cause <see cref="CommandFormatException"/> to be thrown.</param>
    /// <param name="expandHome">If <see langword="true"/>, expands the home directory (by <c>~</c> symbol).</param>
    /// <returns>The expanded path.</returns>
    public static string ExpandVariables(ReadOnlySpan<char> path, bool lenient = false, bool expandHome = true)
    {
        var inVariable = false;

        // If it is solely "~" symbol then do not bother do the rest of the processing and
        // return the HOME.
        if (expandHome && path.Length == 1 && path[0] == '~')
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        
        var builder = new StringBuilder();
        var varBuilder = new StringBuilder();
        var firstChara = true;

        foreach (var x in path)
        {
            // If it is the first character first check if it is "~".
            if (firstChara && expandHome)
            {
                firstChara = false;
                if (x == '~')
                {
                    builder.Append(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    continue;
                }
            }
            
            if (x == '%')
            {
                if (!inVariable)
                {
                    // Begin an environment variable.
                    inVariable = true;
                    continue;
                }

                // End an environment variable.
                inVariable = false;

                // Get variable name and clear builder to reuse.
                var variableName = varBuilder.ToString();
                varBuilder.Clear();

                if (!string.IsNullOrEmpty(variableName))
                {
                    var envVar = Environment.GetEnvironmentVariable(variableName);

                    if (envVar == null && !lenient)
                    {
                        throw ExceptionHelper.UnknownEnvironmentVariable(variableName);
                    }

                    builder.Append(envVar ?? string.Empty);
                    continue;
                }

                // If otherwise, this is hereby escaped.
            }

            if (inVariable)
            {
                varBuilder.Append(x);
            }
            else
            {
                builder.Append(x);
            }
        }

        if (inVariable && !lenient)
        {
            throw ExceptionHelper.WithMessage("generic_env_variable_never_end");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Determines whether the specified shell path points to the root of a file system.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <remarks>
    /// <para>
    /// In Windows, each volume is considered a file system; in POSIX systems, there is only one file system being considered and that
    /// is the root volume.
    /// </para>
    /// </remarks>
    /// <returns><see langword="true"/> if the specified shell path points to the root of a file system; otherwise, <see langword="false"/>.</returns>
    [Obsolete("Use Path.IsPathFullyQualified")]
    public static bool IsRooted(ReadOnlySpan<char> path)
    {
        if (OperatingSystem.IsWindows())
        {
            return char.IsAsciiLetter(path[0]) && path[1] == ':' && path[2] == Path.DirectorySeparatorChar;
        }
        else
        {
            return path[0] == Path.DirectorySeparatorChar;
        }
    }

    /// <summary>
    /// Throws <see cref="CommandFormatException"/> if the specified path is not a valid path to a file.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <exception cref="CommandFormatException">The specified path is invalid.</exception>
    [PublicAPI]
    public static void EnsureFileName(string path)
    {
        CheckPath(path);

        if (Path.IsPathFullyQualified(path))
        {
            return;
        }

        // This is some clean and precise ;)
        if (Array.Exists(Path.GetInvalidFileNameChars(), path.Contains))
        {
            throw ExceptionHelper.InvalidPath();
        }
    }

    /// <summary>
    /// Gets the name of the executable file of the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The name of the executable file, or <see langword="null" /> if none were found.</returns>
    /// <remarks>
    /// <para>
    /// On Windows, this command searches for an executable based on the following order (of extension):
    /// <list type="bullet">
    ///     <item><c>.exe</c></item>
    ///     <item><c>.com</c></item>
    /// </list>
    /// If none of above exists, this method returns <see langword="null" /> (as files without extension names
    /// are not considered executable type in Windows).
    /// </para>
    /// <para>
    /// On all other operating systems, this method returns the name exactly.
    /// </para>
    /// </remarks>
    public static string? GetSystemExecutableName(string name)
    {
        if (OperatingSystem.IsWindows() && !Path.HasExtension(name))
        {
            var exeFile = string.Format(ExeFileExtensionFormat, name);
            var comFile = string.Format(ComFileExtensionFormat, name);

            if (File.Exists(exeFile))
            {
                return exeFile;
            }

            if (File.Exists(comFile))
            {
                return comFile;
            }

            return null;
        }

        return name;
    }

    /// <summary>
    /// Searches for an executable in <c>PATH</c>.
    /// </summary>
    /// <param name="name">The name of the executable to search.</param>
    /// <returns>The first executable found.</returns>
    public static string? SearchExecutable(string name)
    {
        EnsureFileName(name);

        return (from path in Paths where Directory.Exists(path)
            select GetSystemExecutableName(Path.Combine(path, name)))
            .FirstOrDefault(possible => File.Exists(possible)
                                        && FileUtility.IsExecutable(possible));
    }

    /// <summary>
    /// Determines whether the specified path is qualified.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns><see langword="true"/> if the specified path points to a full path; otherwise, <see langword="false"/>.</returns>
    public static bool IsQualifiedPath(string path)
    {
        return path.StartsWith('~') || path.StartsWith('.') || Path.IsPathFullyQualified(path);
    }

    // public static string? SearchExecuteItem(string name, out bool isExecutable)
    // {
    //     EnsureFileName(name);
    // }
}
