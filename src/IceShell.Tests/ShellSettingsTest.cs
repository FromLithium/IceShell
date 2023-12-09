namespace IceShell.Tests;

using IceShell.Core.Settings;
using IniParser.Model;

public class ShellSettingsTest
{
    [Test]
    public void LoadFrom_EmptyNoException()
    {
        var iniData = new IniData();

        Assert.DoesNotThrow(() => ShellSettings.LoadFrom(iniData));
    }
}
