namespace IceShell.Tests;

using IceShell.Core.Exceptions;
using IceShell.Core.FileSystem;
using NexusKrop.IceShell.Core.FileSystem;

public class PathSearcherTests
{
    [Test]
    public void ExpandPath_Variable()
    {
        var testVar = "ICESHELL_TEST_000_EXPANDPATH";
        var testName = "IceShell000Test";

        Environment.SetEnvironmentVariable(testName, testVar);

        var expanded = PathSearcher.ExpandVariables("%IceShell000Test%");

        Assert.That(expanded, Is.EqualTo(testVar));
    }

    [Test]
    public void ExpandPath_Escape()
    {
        Assert.That(PathSearcher.ExpandVariables("%%"),
            Is.EqualTo("%"));
    }

    [Test]
    public void ExpandPath_Complex()
    {
        var testVar = "ICESHELL_TEST_001_COMPLEX";
        var testName = "IceShell001Test";

        Environment.SetEnvironmentVariable(testName, testVar);

        Assert.That(PathSearcher.ExpandVariables("%IceShell001Test%%%"),
            Is.EqualTo("ICESHELL_TEST_001_COMPLEX%"));
    }

    [Test]
    public void ExpandPath_NonLenient_PathNeverEnds()
    {
        Assert.Throws<CommandFormatException>(() => PathSearcher.ExpandVariables("%"));
    }

    [Test]
    public void ExpandPath_NonLenient_NonExistingVariable()
    {
        Environment.SetEnvironmentVariable("IceShell002Test", null);

        Assert.Throws<CommandFormatException>(() => PathSearcher.ExpandVariables("%IceShell002Test%", false));
    }

    [Test]
    [Platform("Win")]
    public void CheckPath_Windows()
    {
        Assert.Throws<CommandFormatException>(() => PathSearcher.CheckPath("path|"));
    }
}
