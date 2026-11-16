using System.Runtime.Versioning;
using IceShell.Tests.Utilities;
using NexusKrop.IceShell.Core.FileSystem;

namespace IceShell.Tests;

public class FileUtilityTests
{
    [Fact]
    public void IsExecutable_NonExistent_False()
    {
        // Arrange
        string testDir = Path.Combine(Path.GetTempPath(),
            "ICESHELL_TEST",
            $"NotxExists_{Path.GetRandomFileName()}");
        string testFile = Path.Combine(testDir, "NON_EXISTS");

        // Act
        bool result = FileUtility.IsExecutable(testFile);

        // Assert
        Assert.False(result);
    }

    [WindowsFact]
    public void IsExecutableWindows_ExecutableFile_True()
    {
        // Arrange
        string file = Environment.GetCommandLineArgs()[0];

        // Act
        bool result = FileUtility.IsExecutable(file);

        // Assert
        Assert.True(result);
    }

    [WindowsFact]
    public void IsExecutableWindows_NonExecutableFile_False()
    {
        // Arrange
        string testDir = Path.Combine(Path.GetTempPath(),
            "ICESHELL_TEST",
            $"NonEx_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(testDir);

        string testFile = Path.Combine(testDir, Path.GetRandomFileName());
        File.WriteAllText(testFile, "Rest assured, this is NOT an executable.");

        // Act
        bool result = FileUtility.IsExecutable(testFile);

        // Assert
        Assert.False(result);
    }

    [UnixFact]
    [UnsupportedOSPlatform("windows")]
    public void IsExecutableUnix_ExecuteBitSet_True()
    {
        // Arrange
        string testDir = Path.Combine(Path.GetTempPath(),
            "ICESHELL_TEST",
            $"UnixMode_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(testDir);

        string testFile = Path.Combine(testDir, Path.GetRandomFileName());
        File.WriteAllText(testFile, "#!/bin/sh\necho Howdy!");
        File.SetUnixFileMode(testFile,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);

        // Act
        bool result = FileUtility.IsExecutable(testFile);

        // Assert
        Assert.True(result);
    }

    [UnixFact]
    [UnsupportedOSPlatform("windows")]
    public void IsExecutableUnix_ExecuteBitNotSet_False()
    {
        // Arrange
        string testDir = Path.Combine(Path.GetTempPath(),
            "ICESHELL_TEST",
            $"UnixMode_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(testDir);

        string testFile = Path.Combine(testDir, Path.GetRandomFileName());
        File.WriteAllText(testFile, "#!/bin/sh\necho Howdy!");
        File.SetUnixFileMode(testFile,
            UnixFileMode.UserRead | UnixFileMode.UserWrite);

        // Act
        bool result = FileUtility.IsExecutable(testFile);

        // Assert
        Assert.False(result);
    }
}
