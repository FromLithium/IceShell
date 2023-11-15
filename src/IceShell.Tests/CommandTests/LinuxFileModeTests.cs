namespace IceShell.Tests.CommandTests;

using IceShell.Platform.Linux;

public class LinuxFileModeTests
{
    [Test]
    public void TryParseNumericFileMode_BasicFunction()
    {
#pragma warning disable NUnit2045
        Assert.That(ChangeFileModeCommand.TryParseNumericFileMode("777", out var mode));
#pragma warning restore NUnit2045
        Assert.That(mode, Is.EqualTo(UnixFileMode.GroupRead | 
                                     UnixFileMode.GroupWrite | 
                                     UnixFileMode.GroupExecute |
                                     UnixFileMode.OtherRead |
                                     UnixFileMode.OtherWrite |
                                     UnixFileMode.OtherExecute |
                                     UnixFileMode.UserRead |
                                     UnixFileMode.UserWrite |
                                     UnixFileMode.UserExecute));
    }

    [Test]
    public void TryParseSingleDigit_BasicFunction()
    {
#pragma warning disable NUnit2045
        Assert.That(ChangeFileModeCommand.TryParseSingleDigit('6', out var perm));
#pragma warning restore NUnit2045
        Assert.That(perm, Is.EqualTo(SinglePermission.Read | SinglePermission.Write));
    }
}