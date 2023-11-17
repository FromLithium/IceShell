// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Converters;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using Moq;

public class AnnotatedCommandRegistryTests
{
    [CommandInfo("test")]
    internal class TestCommand : IShellCommand
    {
        public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
        {
            return CommandResult.Ok();
        }
    }

    [Test]
    public void ResolveAnnotatedCommand_Simple()
    {
        var registry = new AnnotatedCommandRegistry(new RegistryCommandDispatcher());
        var command = registry.ResolveAnnotatedCommand(typeof(TestCommand), out var provider);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.Identifier, Is.EqualTo("test"));
            Assert.That(command.CustomUsage, Is.Null);
            Assert.That(command.Options, Is.Empty);
            Assert.That(command.Values, Is.Empty);
        });
    }
}