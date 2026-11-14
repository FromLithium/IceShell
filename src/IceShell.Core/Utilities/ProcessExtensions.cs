namespace IceShell.Core.Utilities;

using System.ComponentModel;
using System.Diagnostics;
using Mono.Unix.Native;

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
        if (process.HasExited)
        {
            return false;
        }

        if (OperatingSystem.IsWindows())
        {
            return process.CloseMainWindow();
        }

        if (OperatingSystem.IsLinux()
            || OperatingSystem.IsMacOS())
        {       
            var success = Syscall.kill(process.Id, Signum.SIGTERM) == 0;
            if (!success)
            {
                throw new Win32Exception((int)Syscall.GetLastError());
            }

            return true;
        }

        return false;
    }
}
