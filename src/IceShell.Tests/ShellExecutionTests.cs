// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using NSubstitute;

public class ShellExecutionTests
{
    [Fact]
    public void ShellCommandDocument_NoJump()
    {
        var document = new ShellCommandDocument();
        Assert.Throws<NotSupportedException>(() => document.Jump("blah"));
    }

    [Fact]
    public void ShellCommandDocument_JumpNotSupported()
    {
        var document = new ShellCommandDocument();
        Assert.False(document.IsJumpSupported);
    }

    [Fact]
    public void ShellCommandDocument_Variables()
    {
        var document = new ShellCommandDocument();
        Assert.NotNull(document.Variables);

        document.Variables["test_variable"] = "test";
        Assert.Equal("test", document.Variables["test_variable"]);
    }

    [Fact]
    public void CommandParsingUtil_ExpandContextVariables_NoSuchVariable()
    {
        var dispatcher = new RegistryCommandDispatcher(true);
        EchoCommand.RegisterTo(dispatcher);

        Assert.Throws<CommandFormatException>(() =>
            dispatcher.Execute(Substitute.For<IShell>(), new ShellCommandDocument(), "echo $non-existent$"));
    }
    
    [Fact]
    public void CommandParsingUtil_ExpandContextVariables_HaveVariable()
    {
        var dispatcher = new RegistryCommandDispatcher(true);
        EchoCommand.RegisterTo(dispatcher);

        var document = new ShellCommandDocument();
        document.Variables!["exists"] = "exists";

        var exception = Record.Exception(() =>
            dispatcher.Execute(Substitute.For<IShell>(), document, "echo $exists$"));

        Assert.Null(exception);
    }
}