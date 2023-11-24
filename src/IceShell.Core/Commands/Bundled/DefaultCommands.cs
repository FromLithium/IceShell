// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api.Commands;

/// <summary>
/// Provides easy registry access to default commands.
/// </summary>
public static class DefaultCommands
{
    /// <summary>
    /// Registers the default commands to the specified command dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register the commands to.</param>
    public static void Register(IRegistryCommandDispatcher dispatcher)
    {
        var provider = new BundledCommandsProvider(dispatcher);
        provider.Register();

        EchoCommand.RegisterTo(dispatcher);
        ExitCommand.RegisterTo(dispatcher);
    }
}