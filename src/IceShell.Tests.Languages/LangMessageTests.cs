namespace IceShell.Tests.Languages;

using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Settings;

public class LangMessageTests
{
    [SetUp]
    public void SetUp()
    {
        LanguageService.SetInstanceConfig(new ShellSettings()
        {
            Language = "en"
        });
    }

    [Test]
    public void Message_GetTest()
    {
        Assert.That(LangMessage.Get("command.echo.description"), 
            Is.EqualTo("Displays messages."));
    }
    
    [Test]
    public void Message_FromErrorCode()
    {
        Assert.That(LangMessage.MsgFromErrorCode(CommandErrorCode.GenericCommandFail),
            Is.EqualTo("Command failed"));
    }
}