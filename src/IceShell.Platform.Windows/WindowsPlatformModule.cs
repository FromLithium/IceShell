// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

public class WindowsPlatformModule : IModule
{
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        var registry = new AnnotatedCommandRegistry(dispatcher);

        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        registry.Register(typeof(VolumeInfoCommand));
        registry.Register(typeof(MemoryInfoCommand));
    }
}
