namespace IceShell.Tests.CommandTests;

using IceShell.Core;
using IceShell.Core.Commands.Bundled;
using System;
using IceShell.Core.Api;
using IceShell.Core.Commands.Registry;
using NSubstitute;


public class SetCommandTest
{
    private const string TestVarName = "ICESHELL_DEV_TEST_SET_COMMAND";
    private const string TestVarValue = "ICESHELL_DEV_TEST_VALUE";

    [Fact]
    public void SetCommand_SetEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(TestVarName, null);
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);

        Assert.Multiple(() => Assert.True(CommandResult.IsSuccess(dispatcher.Execute(Substitute.For<IShell>(), new ShellCommandDocument(), $"set %{TestVarName}% {TestVarValue}"))),
            () => Assert.Equal(TestVarValue, Environment.GetEnvironmentVariable(TestVarName)));
    }
    
    [Fact]
    public void SetCommand_DeleteEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(TestVarName, TestVarValue);
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);

        Assert.Multiple(
            () => Assert.True(CommandResult.IsSuccess(dispatcher.Execute(Substitute.For<IShell>(), new ShellCommandDocument(), $"set %{TestVarName}="))),
            () => Assert.Null(Environment.GetEnvironmentVariable(TestVarName)));
    }

    [Fact]
    public void SetCommand_SetContextVariable()
    {
        var document = new ShellCommandDocument();
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);
        
        Assert.Multiple(
            () => Assert.True(CommandResult.IsSuccess(dispatcher.Execute(Substitute.For<IShell>(),document, $"set ${TestVarName}$ {TestVarValue}"))),
            () => Assert.Equal(TestVarValue, document.Variables![TestVarName])
        );
    }
    
    [Fact]
    public void SetCommand_DeleteContextVariable()
    {
        var document = new ShellCommandDocument();
        document.Variables![TestVarName] = TestVarValue;
        
        var dispatcher = new RegistryCommandDispatcher();
        SetCommand.RegisterTo(dispatcher);
        
        Assert.Multiple(
            () => Assert.True(CommandResult.IsSuccess(dispatcher.Execute(Substitute.For<IShell>(), document, $"set ${TestVarName}="))),
            () => Assert.False(document.Variables.ContainsKey(TestVarName))
        );
    }
}
