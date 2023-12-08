// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Utilities;

using IceShell.Core.Api;

/// <summary>
/// Provides wrapper services for annotated commands.
/// </summary>
internal class CommandDocumentAnnotationWrapper : ICommandExecutor
{
    private readonly ICommandDocument _document;

    internal CommandDocumentAnnotationWrapper(ICommandDocument document)
    {
        _document = document;
    }
    
    public bool SupportsJump => _document.IsJumpSupported;
    
    public void Jump(string label)
    {
        _document.Jump(label);
    }
}