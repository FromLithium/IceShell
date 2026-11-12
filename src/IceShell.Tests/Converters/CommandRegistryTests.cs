// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Converters;

using IceShell.Core;
using IceShell.Core.Api;
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

    [Fact]
    public void Register_AnnotatedCommandTest()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(TestCommand));
        
        Assert.True(dispatcher.AnyCommand("test"));
    }
    
    [Fact]
    public void Registry_AliasCaseInsensitive()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(TestCommand));
        Assert.Multiple(
            () => Assert.True(dispatcher.HasAlias("TEST")),
            () => Assert.True(dispatcher.HasAlias("TeSt")),
            () => Assert.True(dispatcher.HasAlias("test"))
        );
    }
    
    [Fact]
    public void Registry_IdentifierCaseSensitive()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(TestCommand));
        Assert.Multiple(
            () => Assert.False(dispatcher.AnyCommand("TEST")),
            () => Assert.False(dispatcher.AnyCommand("TeSt")),
            () => Assert.True(dispatcher.AnyCommand("test"))
        );
    }

    [Fact]
    public void ResolveAnnotatedCommand_Simple()
    {
        var registry = new AnnotatedCommandRegistry(new RegistryCommandDispatcher());
        var command = registry.ResolveAnnotatedCommand(typeof(TestCommand), out var provider);
        
        Assert.Multiple(
            () => Assert.Equal("test", command.Identifier),
            () => Assert.Null(command.CustomUsage),
            () => Assert.Empty(command.Options),
            () => Assert.Empty(command.Values));
    }

    [Fact]
    public void Registry_SimpleRegularCommandTest()
    {
        var dispatcher = new RegistryCommandDispatcher();
        var commandInfo = new CommandRegistryEntry("regular");
        
        dispatcher.Register(commandInfo, (_, _, _) => CommandResult.Ok());
        
        Assert.Multiple(
            () => Assert.True(dispatcher.AnyCommand("regular")),
            () => Assert.True(dispatcher.HasAlias("REGULAR")));
    }
}