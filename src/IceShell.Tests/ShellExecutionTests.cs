// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Exceptions;
using Moq;

public class ShellExecutionTests
{
    [Test]
    public void ShellCommandDocument_NoJump()
    {
        var document = new ShellCommandDocument();
        Assert.Throws<NotSupportedException>(() => document.Jump("blah"));
    }

    [Test]
    public void ShellCommandDocument_JumpNotSupported()
    {
        var document = new ShellCommandDocument();
        Assert.That(!document.IsJumpSupported);
    }

    [Test]
    public void ShellCommandDocument_Variables()
    {
        var document = new ShellCommandDocument();
        Assert.That(document.Variables, Is.Not.Null);

        document.Variables!["test_variable"] = "test";
        Assert.That(document.Variables["test_variable"], Is.EqualTo("test"));
    }

    [Test]
    public void CommandParsingUtil_ExpandContextVariables_NoSuchVariable()
    {
        var dispatcher = new RegistryCommandDispatcher(true);
        EchoCommand.RegisterTo(dispatcher);

        Assert.Throws<CommandFormatException>(() =>
            dispatcher.Execute(Mock.Of<IShell>(), new ShellCommandDocument(), "echo $non-existent$"));
    }
    
    [Test]
    public void CommandParsingUtil_ExpandContextVariables_HaveVariable()
    {
        var dispatcher = new RegistryCommandDispatcher(true);
        EchoCommand.RegisterTo(dispatcher);

        var document = new ShellCommandDocument();
        document.Variables!["exists"] = "exists";

        Assert.DoesNotThrow(() =>
            dispatcher.Execute(Mock.Of<IShell>(), document, "echo $exists$"));
    }
}