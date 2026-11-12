namespace IceShell.Tests;

using IceShell.Core.Settings;
using IniParser.Model;

public class ShellSettingsTest
{
    [Fact]
    public void LoadFrom_EmptyNoException()
    {
        var iniData = new IniData();

        var exception = Record.Exception(() => ShellSettings.LoadFrom(iniData));

        Assert.Null(exception);
    }
}
