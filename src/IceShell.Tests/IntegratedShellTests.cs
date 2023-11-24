// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands.Bundled;

public class IntegratedShellTests
{
    [Test]
    public void Execute_HelloWorld()
    {
        var shell = new Shell();
        EchoCommand.RegisterTo(shell.Dispatcher);
        var result = shell.Execute("echo Hello World!");
        
        Assert.That(CommandResult.IsSuccess(result));
    }
}