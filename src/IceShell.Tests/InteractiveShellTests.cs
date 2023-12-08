// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;

public class InteractiveShellTests
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
}