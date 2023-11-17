// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Batching;

using IceShell.Batching.Commands;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

public class BatchingModule : IModule
{
    public void Initialize(IRegistryCommandDispatcher dispatcher)
    {
        var registry = new AnnotatedCommandRegistry(dispatcher);
        
        registry.Register(typeof(GotoCommand));
        registry.Register(typeof(CallCommand));
    }
}
