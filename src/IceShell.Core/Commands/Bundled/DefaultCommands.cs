// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Compat;
using IceShell.Core.Commands.Simple;

/// <summary>
/// Provides easy registry access to default commands.
/// </summary>
public static class DefaultCommands
{
    /// <summary>
    /// Registers the default commands to the specified simple command dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register the commands to.</param>
    public static void Register(ISimpleCommandDispatcher dispatcher)
    {
        AliasCommand.RegisterTo(dispatcher);
        UnaliasCommand.RegisterTo(dispatcher);

        Register(new RegistryCommandRegistrationBridge(dispatcher));
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
    }
}