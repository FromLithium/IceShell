// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using NexusKrop.IceCube.Util.Enumerables;
using NexusKrop.IceShell.Core.Commands.Bundled;

/// <summary>
/// Provides integration of bundled commands. This class cannot be inherited.
/// </summary>
public sealed class BundledCommandsProvider
{
    private readonly AnnotatedCommandRegistry _registry;

    private static readonly Type[] BundledCommands =
    {
        typeof(DirCommandEx),
        typeof(CdCommandEx),
        typeof(CopyCommandEx),
        typeof(StartCommandEx),
        typeof(ClearScreenCommandEx),
        typeof(MakeDirectoryCommand),
        typeof(DeleteFileCommandEx),
        typeof(MakeFileCommand),
        typeof(MoveCommandEx),
        typeof(TypeCommandEx),
        typeof(PromptCommandEx),
        typeof(HelpCommandEx),
        typeof(FindCommand),
        typeof(DateCommand)
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="BundledCommandsProvider"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register the bundled commands to.</param>
    public BundledCommandsProvider(IRegistryCommandDispatcher dispatcher)
    {
        _registry = new AnnotatedCommandRegistry(dispatcher);
    }

    /// <summary>
    /// Registers all bundled commands with the associated dispatcher.
    /// </summary>
    public void Register()
    {
        BundledCommands.ForEach(_registry.Register);
    }
}