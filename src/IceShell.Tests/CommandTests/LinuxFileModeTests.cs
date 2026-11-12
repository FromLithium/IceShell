namespace IceShell.Tests.CommandTests;

using System.Runtime.Versioning;
using IceShell.Platform.Linux;
using IceShell.Tests.Utilities;

[UnsupportedOSPlatform("windows")]
public class LinuxFileModeTests
{
    [UnixFact]
    public void TryParseNumericFileMode_BasicFunction()
    {
        Assert.True(ChangeFileModeCommand.TryParseNumericFileMode("777", out var mode));
        const UnixFileMode expected = UnixFileMode.GroupRead |
                                     UnixFileMode.GroupWrite |
                                     UnixFileMode.GroupExecute |
                                     UnixFileMode.OtherRead |
                                     UnixFileMode.OtherWrite |
                                     UnixFileMode.OtherExecute |
                                     UnixFileMode.UserRead |
                                     UnixFileMode.UserWrite |
                                     UnixFileMode.UserExecute;
        Assert.Equal(expected, mode);
    }

    [Fact]
    public void TryParseSingleDigit_BasicFunction()
    {
#pragma warning disable NUnit2045
        Assert.True(ChangeFileModeCommand.TryParseSingleDigit('6', out var perm));
#pragma warning restore NUnit2045
        Assert.Equal(SinglePermission.Read | SinglePermission.Write,
            perm);
    }
}