// SPDX-FileCopyrightText: 2023, 2026 WithLithum
// SPDX-License-Identifier: GPL-3.0-or-later

using IceShell.Core.Api;
using IceShell.Core.Commands.Registry;

namespace IceShell.Tests.Converters;

public class CommandRegistryTests
{
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
