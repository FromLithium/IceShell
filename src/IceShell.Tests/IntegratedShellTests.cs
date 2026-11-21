// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core;
using IceShell.Core.Api;

public class IntegratedShellTests
{
    [Fact]
    public void Execute_HelloWorld()
    {
        var shell = new Shell();
        var result = shell.Execute("echo Hello World!");

        Assert.True(CommandResult.IsSuccess(result));
    }
}
