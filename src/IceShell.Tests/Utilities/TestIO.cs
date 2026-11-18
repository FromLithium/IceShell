using System.Diagnostics;

namespace IceShell.Tests.Utilities;

internal static class TestIO
{
    internal static void MakeMockExecutable(string fileName)
    {
        File.WriteAllText(fileName, """
        #!/bin/sh
        echo Executable!
        """);

        MarkExecuteOnUnix(fileName);
    }

    internal static void MarkExecuteOnUnix(string fileName)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsBrowser())
        {
            return;
        }

        File.SetUnixFileMode(fileName,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
    }

    internal static string GetTestDir(string testType)
    {
        const int attempts = 10;
        string tempPath = Path.GetTempPath();
        for (int i = 0; i < attempts; i++)
        {
            string testDir = Path.Combine(tempPath, nameof(IceShell),
                "__TEST__",
                $"{testType}__{Path.GetRandomFileName()}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
                return testDir;
            }

            Debug.WriteLine($"WARN: attempt #{i} make tempdir for '${testType}'");
        }

        throw new IOException($"Failed to create temp directory for test '{testType}'" +
            $"(reached max {attempts} tries)");
    }
}
