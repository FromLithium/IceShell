// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Converters;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;

public class CommandRegistryTests
{
    [CommandInfo("test")]
    private class TestCommand : IShellCommand
    {
        public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
        {
            return CommandResult.Ok();
        }
    }

    [Test]
    public void Register_AnnotatedCommandTest()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(TestCommand));
        
        Assert.That(dispatcher.AnyCommand("test"));
    }
    
    [Test]
    public void Registry_AliasCaseInsensitive()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(TestCommand));
        Assert.Multiple(() =>
        {
            Assert.That(dispatcher.HasAlias("TEST"));
            Assert.That(dispatcher.HasAlias("TeSt"));
            Assert.That(dispatcher.HasAlias("test"));
        });
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

    [Test]
    public void Registry_SimpleRegularCommandTest()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var commandInfo = new CommandRegistryEntry("regular");
        
        dispatcher.Register(commandInfo, (_, _, _) => CommandResult.Ok());
        
        Assert.Multiple(() =>
        {
            Assert.That(dispatcher.AnyCommand("regular"));
            Assert.That(dispatcher.HasAlias("REGULAR"));
        });
    }
}