// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;

public class WindowsPlatformModule : IModule
{
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        
        MemoryInfoCommand.RegisterTo(dispatcher);
        VolumeInfoCommand.RegisterTo(dispatcher);
        WindowsClipCommand.RegisterTo(dispatcher);
    }
}
