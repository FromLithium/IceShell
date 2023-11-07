// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;

using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Commands.Bundled;
using global::IceShell.Core.Api;

/// <summary>
/// Copies a file to another location.
/// </summary>
[CommandInfo("copy", CustomUsage = "<sources...> <destination>")]
[VariableValue]
public class CopyCommandEx : DestinationFileCommandBase
{
    /// <inheritdoc />
    public override void DoOperation(string source, string destination)
    {
        File.Copy(source, destination);
    }
}
