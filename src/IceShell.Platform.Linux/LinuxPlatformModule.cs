using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IceShell.Tests")]

namespace IceShell.Platform.Linux;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

public class LinuxPlatformModule : IModule
{
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        if (OperatingSystem.IsLinux())
        {
            registry.Register(typeof(ChangeFileModeCommand));
        }
    }
}
