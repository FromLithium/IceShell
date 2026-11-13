namespace IceShell.Core.Utilities;

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tmds.Linux;

/// <summary>
/// Provides extension methods for process objects.
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    /// Instructs the specified process to perform graceful termination.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <returns><see langword="true"/> if the signal can be delivered; otherwise, <see langword="false"/>.</returns>
    public static bool Terminate(this Process process)
    {
        if (OperatingSystem.IsWindows())
        {
            return process.CloseMainWindow();
        }

        if (OperatingSystem.IsLinux())
        {       
            var success = LibC.kill(process.Id, 15 /* SIGTERM */) == 0;
            if (!success)
            {
                throw new Win32Exception(LibC.errno);
            }
        }

        return false;
    }
}
