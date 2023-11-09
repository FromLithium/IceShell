// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Native;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
internal static partial class WindowsFileApi
{
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial uint GetShortPathNameW(string lpszLongPath, char[] lpszShortName, uint cchBuffer);

    public static string GetShortPathName(string longPath)
    {
        var length = GetShortPathNameW(longPath, Array.Empty<char>(), 0);

        var result = new char[length];

        var retVal = GetShortPathNameW(longPath, result, length);

        if (retVal == 0)
        {
            return longPath;
        }

        return new string(result);
    }
}
