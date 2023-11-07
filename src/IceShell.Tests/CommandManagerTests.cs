// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using NexusKrop.IceShell.Core.Commands;
using NexusKrop.IceShell.Core.Commands.Bundled;

public class CommandManagerTests
{
    /// <summary>
    /// A test that assures the behaviour of the <see cref="CommandManager.HasDefinition(string)"/> method.
    /// </summary>
    [Test]
    public void HasDefinition_BasicFeature()
    {
        var manager = new CommandManager(false);
        manager.Register(typeof(EchoCommandEx));

        Assert.Multiple(() =>
        {
            Assert.That(manager.HasDefinition("echo"), Is.True);
            Assert.That(manager.HasDefinition("eChO"), Is.True);
            Assert.That(manager.HasDefinition("ECHO"), Is.True);
        });
    }

    /// <summary>
    /// A test that assures <see cref="CommandManager.HasAlias(string)"/> does not care about the existence
    /// of the command that the alias points to.
    /// </summary>
    [Test]
    public void HasAlias_DoesNotCareAboutCommandExistence()
    {
        var manager = new CommandManager(true);
        manager.RegisterAlias("ALIAS", "fake");

        Assert.Multiple(() =>
        {
            Assert.That(manager.HasAlias("ALIAS"), Is.True);
            Assert.That(manager.HasDefinition("ALIAS"), Is.False);
        });
    }

    /// <summary>
    /// A test that assures the command aliases are case insensitive.
    /// </summary>
    [Test]
    public void Aliases_AreCaseInsensitive()
    {
        var manager = new CommandManager(false);
        manager.RegisterAlias("aLiAs", "command");

        Assert.Multiple(() =>
        {
            Assert.That(manager.HasAlias("AlIaS"));
            Assert.That(manager.HasAlias("ALIAS"));
            Assert.That(manager.HasAlias("alias"));
        });
    }

    [Test]
    public void CommandIds_AreCaseSensitive()
    {
        var manager = new CommandManager(false);
        manager.Register(typeof(EchoCommandEx));

        Assert.Multiple(() =>
        {
            Assert.That(manager.HasCommand(EchoCommandEx.Id), Is.True);
            Assert.That(manager.HasCommand(EchoCommandEx.Id.ToUpperInvariant()), Is.False);
            Assert.That(manager.HasCommand("eChO"), Is.False);
        });
    }
}
