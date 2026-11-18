using IceShell.Core.Utilities;
using IceShell.Tests.Utilities;

namespace IceShell.Tests;

public class ExecutableSearcherTest
{
    [Fact]
    public void FindSystem_BareNameNotInCwd_MatchPath()
    {
        // Arrange
        const string fileName = "foo";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string file1 = Path.Combine(tempDir1, fileName);
        string cd = TestIO.GetTestDir("ExSearchPathCd");

        File.WriteAllText(file1, "TEST EXECUTABLE");
        TestIO.MarkExecuteOnUnix(file1);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            currentDirectory: cd);

        // Assert
        Assert.Equal(Path.GetFullPath(fileName, tempDir1),
            result);
    }

    [Fact]
    public void FindSystem_BareNameInCwdOnly_MatchCwd()
    {
        // Arrange
        const string fileName = "foo";

        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            currentDirectory: cd,
            matchCurrentDirectory: true);

        // Assert
        Assert.Equal(cdFile, result);
    }

    [Fact]
    public void FindSystem_BareNameInCwdAndPath_MatchCwd()
    {
        // Arrange
        const string fileName = "foo";

        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string file1 = Path.Combine(tempDir1, fileName);
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(file1);
        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            currentDirectory: cd,
            matchCurrentDirectory: true);

        // Assert
        Assert.Equal(cdFile, result);
    }

    [Fact]
    public void FindSystem_NameWithExtensionInPath_MatchPath()
    {
        // Arrange
        const string fileName = "foo.TEST";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string file1 = Path.Combine(tempDir1, fileName);
        string cd = TestIO.GetTestDir("ExSearchPathCd");

        TestIO.MakeMockExecutable(file1);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            pathExtensions: ".TEST",
            currentDirectory: cd);

        // Assert
        Assert.Equal(file1, result);
    }

    [Fact]
    public void FindSystem_NameWithExtensionInCwd_MatchCwd()
    {
        // Arrange
        const string fileName = "foo.TEST";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");

        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            pathExtensions: ".TEST",
            currentDirectory: cd,
            matchCurrentDirectory: true);

        // Assert
        Assert.Equal(cdFile, result);
    }

    [Fact]
    public void FindSystem_NameWithExtensionInCwdAndPath_MatchCwd()
    {
        // Arrange
        const string fileName = "foo.TEST";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string file1 = Path.Combine(tempDir1, fileName);

        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(file1);
        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            pathExtensions: ".TEST",
            currentDirectory: cd,
            matchCurrentDirectory: true);

        // Assert
        Assert.Equal(cdFile, result);
    }

    [Fact]
    public void FindSystemCwdOff_NameWithExtensionInCwd_MatchNothing()
    {
        // Arrange
        const string fileName = "foo.TEST";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");

        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            pathExtensions: ".TEST",
            currentDirectory: cd,
            matchCurrentDirectory: false);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindSystemCwdOff_NameWithExtensionInCwdAndPath_MatchPath()
    {
        // Arrange
        const string fileName = "foo.TEST";
        string tempDir1 = TestIO.GetTestDir("ExSearchPathTestA");
        string file1 = Path.Combine(tempDir1, fileName);

        string cd = TestIO.GetTestDir("ExSearchPathCd");
        string cdFile = Path.Combine(cd, fileName);

        TestIO.MakeMockExecutable(file1);
        TestIO.MakeMockExecutable(cdFile);

        // Act
        string? result = ExecutableSearcher.FindSystem(fileName,
            path: tempDir1,
            pathExtensions: ".TEST",
            currentDirectory: cd,
            matchCurrentDirectory: false);

        // Assert
        Assert.Equal(file1, result);
    }
}
