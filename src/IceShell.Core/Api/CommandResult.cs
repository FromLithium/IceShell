// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.CLI.Languages;

/// <summary>
/// Represents the result of a command.
/// </summary>
public record struct CommandResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandResult"/> structure.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="outStream">The redirection output stream.</param>
    /// <param name="exception">The exception associated with the result.</param>
    /// <param name="message">The message associated with the result.</param>
    public CommandResult(int exitCode, CommandErrorCode errorCode, TextReader? outStream = null, Exception? exception = null, string? message = null)
    {
        ExitCode = exitCode;
        ErrorCode = errorCode;
        OutStream = outStream;
        Exception = exception;
        Message = message;
    }

    /// <summary>
    /// Gets or sets the exit code returned by the command.
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// Gets or sets the error code returned by the command.
    /// </summary>
    public CommandErrorCode ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the logical output source returned by the command.
    /// </summary>
    public TextReader? OutStream { get; set; }

    /// <summary>
    /// Gets or sets the exception associated with the execution result.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the execution result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Determines whether the specified command result is of a successful status.
    /// </summary>
    /// <param name="result">The result to determine.</param>
    /// <returns><see langword="true"/> if the result represents successful status; otherwise, <see langword="false"/>.</returns>
    public static bool IsSuccess(CommandResult result)
    {
        return result.Exception == null
               && result.ErrorCode == CommandErrorCode.None
               && result.ExitCode == 0;
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> with successful exit code, and optionally
    /// with a redirection output.
    /// </summary>
    /// <param name="outStream">The redirection output.</param>
    /// <returns>A successful result.</returns>
    public static CommandResult Ok(TextReader? outStream = null)
    {
        return new CommandResult(0, CommandErrorCode.None, outStream);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> with an exit code, an error code, and optionally an output stream.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="outStream">The redirection output stream.</param>
    /// <param name="message">The message associated with the result.</param>
    /// <returns>The execution result.</returns>
    public static CommandResult WithCode(int exitCode, CommandErrorCode errorCode = CommandErrorCode.None, TextReader? outStream = null, string? message = null)
    {
        return new CommandResult(exitCode, errorCode, outStream, null, message);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> with an error code and an exception.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="message">The message associated with the result.</param>
    /// <returns>The result with exception.</returns>
    public static CommandResult WithException(CommandErrorCode errorCode, Exception exception, string? message = null)
    {
        return new CommandResult(-1, errorCode, null, exception, message);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> with an error code.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message associated with the result.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithError(CommandErrorCode errorCode, string? message = null)
    {
        return new CommandResult(-2, errorCode, null, null, message);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a directory required was not found.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithBadDirectory(string directory)
    {
        return WithError(CommandErrorCode.BadDirectory, LangMessage.MsgDirectoryNotFound(directory));
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a value is missing.
    /// </summary>
    /// <param name="valuePos">The position of value.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithMissingValue(int valuePos)
    {
        return WithError(CommandErrorCode.SyntaxError, LangMessage.MsgMissingValue(valuePos));
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a value is missing.
    /// </summary>
    /// <param name="valueName">The reference name of value.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithMissingValue(string valueName)
    {
        return WithError(CommandErrorCode.SyntaxError, LangMessage.MsgMissingValue(valueName));
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a required file was not found.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithBadFile(string file)
    {
        return WithError(CommandErrorCode.BadFile, LangMessage.MsgFileNotFound(file));
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a directory required was not found.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithExistingDirectory(string directory)
    {
        return WithError(CommandErrorCode.DirectoryExists, LangMessage.MsgDirectoryAlreadyExists(directory));
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that a file already exists.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The result.</returns>
    public static CommandResult WithExistingFile(string file)
    {
        return WithError(CommandErrorCode.FileExists, LangMessage.MsgFileAlreadyExists(file));
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommandResult"/> indicating that the last operation is interrupted
    /// abnormally.
    /// </summary>
    /// <returns>The created <see cref="CommandResult"/>.</returns>
    public static CommandResult WithInterrupted()
    {
        return CommandResult.WithError(CommandErrorCode.Interrupted);
    }
}
