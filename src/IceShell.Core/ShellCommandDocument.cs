// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core;

using IceShell.Core.Api;

internal class ShellCommandDocument : ICommandDocument
{
    public bool IsJumpSupported => false;

    public void Jump(string label)
    {
        throw new NotSupportedException();
    }

    public IDictionary<string, string>? Variables { get; } = new Dictionary<string, string>();
}