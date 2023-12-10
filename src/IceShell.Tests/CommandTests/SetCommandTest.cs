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
    private const string TestVarName = "ICESHELL_DEV_TEST_SET_COMMAND";
    private const string TestVarValue = "ICESHELL_DEV_TEST_VALUE";

    [Test]
    public void SetCommand_SetEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(TestVarName, null);
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);

        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(dispatcher.Execute(Mock.Of<IShell>(), new ShellCommandDocument(), $"set %{TestVarName}% {TestVarValue}")));
            Assert.That(Environment.GetEnvironmentVariable(TestVarName), Is.EqualTo(TestVarValue));
        });
    }
    
    [Test]
    public void SetCommand_DeleteEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(TestVarName, TestVarValue);
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);

        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(dispatcher.Execute(Mock.Of<IShell>(), new ShellCommandDocument(), $"set %{TestVarName}=")));
            Assert.That(Environment.GetEnvironmentVariable(TestVarName), Is.Null);
        });
    }

    [Test]
    public void SetCommand_SetContextVariable()
    {
        var document = new ShellCommandDocument();
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);
        
        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(dispatcher.Execute(Mock.Of<IShell>(),document, $"set ${TestVarName}$ {TestVarValue}")));
            Assert.That(document.Variables![TestVarName], Is.EqualTo(TestVarValue));
        });
    }
    
    [Test]
    public void SetCommand_DeleteContextVariable()
    {
        var document = new ShellCommandDocument();
        document.Variables![TestVarName] = TestVarValue;
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);
        
        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(dispatcher.Execute(Mock.Of<IShell>(), document, $"set ${TestVarName}=")));
            Assert.That(document.Variables!.ContainsKey(TestVarName), Is.False);
        });
    }
}
