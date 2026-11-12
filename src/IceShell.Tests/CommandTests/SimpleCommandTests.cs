// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.CommandTests;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Registry;
using NSubstitute;

public class SimpleCommandTests
{
    [Fact]
    public void Exit_ShellExit()
    {
        var shell = Substitute.For<IShell>();
        var dispatcher = new RegistryCommandDispatcher();
        ExitCommand.RegisterTo(dispatcher);

        var retVal = dispatcher.Execute(shell, new ShellCommandDocument(), "exit");

        Assert.Multiple(
            () => Assert.True(CommandResult.IsSuccess(retVal)),
            () => shell.Received(requiredNumberOfCalls: 1).Quit());
    }
}