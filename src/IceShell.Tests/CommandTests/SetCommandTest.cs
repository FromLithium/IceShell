namespace IceShell.Tests.CommandTests;

using IceShell.Core;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Bundled;
using Moq;
using System;
using IceShell.Core.Api;
using IceShell.Core.Commands.Registry;

public class SetCommandTest
{
    public const string TestVarName = "ICESHELL_DEV_TEST_SET_COMMAND";

    [Test]
    public void SetCommand_SetVar()
    {
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);

        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(dispatcher.Execute(Mock.Of<IShell>(), new ShellCommandDocument(), $"set {TestVarName} ICESHELL_DEV_TEST_VALUE")));
            Assert.That(Environment.GetEnvironmentVariable(TestVarName), Is.EqualTo("ICESHELL_DEV_TEST_VALUE"));
        });
    }

    [SetUp]
    public void SetUp()
    {
        Environment.SetEnvironmentVariable(TestVarName, null);
    }
}
