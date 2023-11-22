// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.CommandTests;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Registry;
using Moq;

public class SimpleCommandTests
{
    [Test]
    public void Exit_ShellExit()
    {
        var shell = new Mock<IShell>();
        var dispatcher = new RegistryCommandDispatcher();
        ExitCommand.RegisterTo(dispatcher);

        var retVal = dispatcher.Execute(shell.Object, shell.Object, "exit");

        Assert.Multiple(() =>
        {
            Assert.That(CommandResult.IsSuccess(retVal));
            shell.Verify(x => x.Quit(), Times.Once);
        });
    }
}