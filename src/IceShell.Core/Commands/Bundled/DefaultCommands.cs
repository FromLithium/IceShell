// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Compat;
using IceShell.Core.Commands.Internal;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Provides easy registry access to default commands.
/// </summary>
public static class DefaultCommands
{
    private static readonly RegistryCommandRegistrationBridge SimpleBridge = CreateSimpleBridge();

    internal static IReadOnlyDictionary<string, InternalCommand> SimpleCommands => SimpleBridge.Commands;

    internal static bool TryGetCommandInfo(string name, out CommandRegistryEntry? info)
    {
        return SimpleBridge.TryGetEntry(name, out info);
    }

    private static RegistryCommandRegistrationBridge CreateSimpleBridge()
    {
        var bridge = new RegistryCommandRegistrationBridge();
        Register(bridge);
        return bridge;
    }

    /// <summary>
    /// Registers the default commands to the specified command dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register the commands to.</param>
    public static void Register(IRegistryCommandDispatcher dispatcher)
    {
        EchoCommand.RegisterTo(dispatcher);
        ExitCommand.RegisterTo(dispatcher);
        NewIfCommand.RegisterTo(dispatcher);
        MoreCommand.RegisterTo(dispatcher);
        SetCommand.RegisterTo(dispatcher);
        RemarksCommand.RegisterTo(dispatcher);
        ShellVersionCommand.RegisterTo(dispatcher);
        HelpCommand.RegisterTo(dispatcher);
        ClearScreenCommand.RegisterTo(dispatcher);
        CdCommand.RegisterTo(dispatcher);
        CopyCommand.RegisterTo(dispatcher);
        MoveCommand.RegisterTo(dispatcher);
        PromptCommand.RegisterTo(dispatcher);
        StartCommand.RegisterTo(dispatcher);
        MakeDirectoryCommand.RegisterTo(dispatcher);
        MakeFileCommand.RegisterTo(dispatcher);
        DirCommand.RegisterTo(dispatcher);
        DeleteFileCommand.RegisterTo(dispatcher);
        DateCommand.RegisterTo(dispatcher);
        TypeCommand.RegisterTo(dispatcher);
        FindCommand.RegisterTo(dispatcher);
        CallCommand.RegisterTo(dispatcher);
        GotoCommand.RegisterTo(dispatcher);
    }
}
