// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.FileSystem;

using System;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using Mono.Unix.Native;

/// <summary>
/// Provides utilities for file formats and types.
/// </summary>
public static class FileUtility
{
    /// <summary>
    /// Determines whether the specified file is an executable.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="noPeCheck">
    /// If <see langword="true"/>, on Windows, always return <see langword="true"/> if the file
    /// exists.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the specified file exists and is executable; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool IsExecutable(string file,
        bool noPeCheck = false)
    {
        if (!File.Exists(file))
        {
            return false;
        }

        if (OperatingSystem.IsWindows())
        {
            return noPeCheck || IsExecutableCoreWindows(file);
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return IsExecutableCoreUnix(file);
        }
        else
        {
            // This usually means we are in WASM.
            //
            // Elaborate test for whether the file is an executable will mean a lot of work so even
            // if we are in WASI, the DOS behaviour of "Shell Execute if Not Executable" is not
            // really viable here.
            return false;
        }
    }

    [UnsupportedOSPlatform("windows")]
    private static bool IsExecutableCoreUnix(string fileName)
    {
        var success = Syscall.access(fileName, AccessModes.X_OK) == 0;
        if (!success)
        {
            Errno errno = Stdlib.GetLastError();
            if (errno == Errno.EACCES)
            {
                return false;
            }

            throw new Win32Exception((int)errno);
        }

        return true;
    }

    [SupportedOSPlatform("windows")]
    private static bool IsExecutableCoreWindows(string fileName)
    {
        try
        {
            using Stream stream = File.OpenRead(fileName);
            var reader = new PEReader(stream);

            if (reader.PEHeaders == null)
            {
                return false;
            }

            return reader.PEHeaders.IsExe;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }
}
