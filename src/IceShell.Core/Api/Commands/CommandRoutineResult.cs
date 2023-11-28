// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

/// <summary>
/// Defines a structure for returning either an error or the resulting value in intermediate routines
/// called within command processing logic routines.
/// </summary>
/// <typeparam name="T">The type of the execution result.</typeparam>
public struct CommandRoutineResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRoutineResult{T}"/> structure with the specified error.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    public CommandRoutineResult(CommandErrorCode errorCode)
    {
        Result = default;
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandRoutineResult{T}"/> structure with the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public CommandRoutineResult(T result)
    {
        Result = result;
        ErrorCode = CommandErrorCode.None;
    }
    
    /// <summary>
    /// Gets the result of the execution.
    /// </summary>
    public T? Result { get; }

    /// <summary>
    /// Gets the error code.
    /// </summary>
    public CommandErrorCode ErrorCode { get; }

    /// <summary>
    /// Converts the specified result to the its representation in <see cref="CommandRoutineResult{T}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The <see cref="CommandRoutineResult{T}"/> instance.</returns>
    public static implicit operator CommandRoutineResult<T>(T value)
    {
        return new CommandRoutineResult<T>(value);
    }

    /// <summary>
    /// Converts the specified error code to its representation in <see cref="CommandRoutineResult{T}"/>.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <returns>The &lt;see cref="CommandRoutineResult{T}"/&gt; instance.</returns>
    public static implicit operator CommandRoutineResult<T>(CommandErrorCode errorCode)
    {
        return new CommandRoutineResult<T>(errorCode);
    }
}