// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

/// <summary>
/// An enumeration of error codes of internal commands.
/// </summary>
public enum CommandErrorCode
{
    /// <summary>
    /// No error have occurred. For external commands, also check the exit code.
    /// </summary>
    None,
    /// <summary>
    /// Failed to parse the command. This does not usually indicate a syntax error, but rather something is wrong with the parsing routine.
    /// </summary>
    ParsingFailure,
    /// <summary>
    /// Failed to start an external process.
    /// </summary>
    ExternalStartFail,
    /// <summary>
    /// The command have failed without a specified reason.
    /// </summary>
    GenericCommandFail,
    /// <summary>
    /// The command have failed with an exception but without a specified error code.
    /// </summary>
    GenericCommandException,
    /// <summary>
    /// The operation requires elevated privileges (also known as <c>root</c> privileges on Unix systems).
    /// </summary>
    ElevationRequired,
    /// <summary>
    /// Directory not found.
    /// </summary>
    BadDirectory,
    /// <summary>
    /// A file have been specified as the destination for a multi-file operation, but the source contains multiple files.
    /// </summary>
    SingleDestinationMultiSource,
    /// <summary>
    /// Incorrect command syntax.
    /// </summary>
    SyntaxError,
    /// <summary>
    /// File not found.
    /// </summary>
    BadFile,
    /// <summary>
    /// Invalid argument.
    /// </summary>
    BadArgument,
    /// <summary>
    /// An error associated with the operating system.
    /// </summary>
    OperatingSystemError,
    /// <summary>
    /// The operation is not supported.
    /// </summary>
    OperationNotSupported,
    /// <summary>
    /// The file already exists.
    /// </summary>
    FileExists,
    /// <summary>
    /// The directory already exists.
    /// </summary>
    DirectoryExists,
    /// <summary>
    /// The current user does not have the permissions to write to the specified destination.
    /// </summary>
    WriteUnauthorized,
    /// <summary>
    /// Requires pipeline input.
    /// </summary>
    RequiresPipelineInput,
    /// <summary>
    /// Command not found.
    /// </summary>
    BadCommand,
    /// <summary>
    /// Pipe redirection to the current command is not supported.
    /// </summary>
    PipelineToNotSupported,
    /// <summary>
    /// Operation is cancelled or interrupted.
    /// </summary>
    Interrupted
}
