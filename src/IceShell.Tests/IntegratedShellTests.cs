// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands.Bundled;

public class IntegratedShellTests
{
    [Fact]
    public void Execute_HelloWorld()
    {
        var shell = new Shell();
        EchoCommand.RegisterTo(shell.Dispatcher);
        var result = shell.Execute("echo Hello World!");
        
        Assert.True(CommandResult.IsSuccess(result));
    }

    [Fact]
    public void ModuleManager_NotNull()
    {
        var shell = new Shell();
        Assert.NotNull(shell.ModuleManager);
    }
}