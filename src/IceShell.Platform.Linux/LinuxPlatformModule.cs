using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IceShell.Tests")]

namespace IceShell.Platform.Linux;

using IceShell.Core.Api;

public class LinuxPlatformModule : IModule
{
    public void Initialize(ICommandDispatcher dispatcher)
    {
        if (OperatingSystem.IsLinux())
        {
            dispatcher.CommandManager.Register(typeof(ChangeFileModeCommand));
        }
    }
}
