// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using IceShell.Core.Api;

public class WindowsPlatformModule : IModule
{
    public void Initialize(ICommandDispatcher dispatcher)
    {
        dispatcher.CommandManager.Register(typeof(VolumeInfoCommand));
        dispatcher.CommandManager.Register(typeof(MemoryInfoCommand));
    }
}
