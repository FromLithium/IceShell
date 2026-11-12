namespace IceShell.Tests;

using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using IceShell.Tests.Utilities;
using NexusKrop.IceShell.Core.FileSystem;

public class PathSearcherTests
{
    [Fact]
    public void ExpandPath_Variable()
    {
        var testVar = "ICESHELL_TEST_000_EXPANDPATH";
        var testName = "IceShell000Test";

        Environment.SetEnvironmentVariable(testName, testVar);

        var expanded = PathSearcher.ExpandVariables("%IceShell000Test%");

        Assert.Equal(expanded, testVar);
    }

    [Fact]
    public void ExpandHome_Basic()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var separator = Path.DirectorySeparatorChar;
        var basePath = $"Documents{separator}Test.txt";
        var combined = Path.Combine(home, basePath);
        
        Assert.Equal(PathSearcher.ExpandHome($"~{basePath}"),
            combined);
    }

    [Fact]
    public void ExpandHome_NoExpandMiddle()
    {
        var separator = Path.DirectorySeparatorChar;
        var testPath = $"{separator}~{separator}test";
        
        Assert.Equal(PathSearcher.ExpandHome(testPath), testPath);
    }

    [Fact]
    public void ExpandHome_JustHome()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Assert.Equal(PathSearcher.ExpandHome("~"), home);
    }

    [Fact]
    public void ExpandPath_Escape()
    {
        Assert.Equal(PathSearcher.ExpandVariables("%%"), "%");
    }

    [Fact]
    public void ExpandPath_Complex()
    {
        var testVar = "ICESHELL_TEST_001_COMPLEX";
        var testName = "IceShell001Test";

        Environment.SetEnvironmentVariable(testName, testVar);

        Assert.Equal("ICESHELL_TEST_001_COMPLEX%", PathSearcher.ExpandVariables("%IceShell001Test%%%"));
    }

    [Fact]
    public void ExpandPath_NonLenient_PathNeverEnds()
    {
        Assert.Throws<CommandFormatException>(() => PathSearcher.ExpandVariables("%"));
    }

    [Fact]
    public void ExpandPath_NonLenient_NonExistingVariable()
    {
        Environment.SetEnvironmentVariable("IceShell002Test", null);

        Assert.Throws<CommandFormatException>(() => PathSearcher.ExpandVariables("%IceShell002Test%", false));
    }

    [WindowsFact]
    public void CheckPath_Windows()
    {
        Assert.Throws<CommandFormatException>(() => PathSearcher.CheckPath("path|"));
    }
}
