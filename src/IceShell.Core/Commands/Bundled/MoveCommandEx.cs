// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace NexusKrop.IceShell.Core.Commands.Bundled;
using global::IceShell.Core.Commands.Attributes;
using global::IceShell.Core.Commands.Bundled;
using global::IceShell.Core.Api;

/// <summary>
/// Defines a command that moves or renames a file.
/// </summary>
[CommandInfo("move")]
[VariableValue]
public class MoveCommandEx : DestinationFileCommandBase
{
    /// <inheritdoc />
    public override void DoOperation(string source, string destination)
    {
        File.Move(source, destination);
    }
}
