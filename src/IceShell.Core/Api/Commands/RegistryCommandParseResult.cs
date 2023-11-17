// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api.Commands;

using IceShell.Core.Commands;

/// <summary>
/// Represents the parsing result of a command.
/// </summary>
public struct RegistryCommandParseResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryCommandParseResult"/> structure.
    /// </summary>
    /// <param name="commandId">The ID of the command to execute.</param>
    /// <param name="arguments">The arguments supplied.</param>
    public RegistryCommandParseResult(string commandId, CommandArgumentResult arguments)
    {
        CommandId = commandId;
        Arguments = arguments;
    }
    
    /// <summary>
    /// Gets the identifier of the command to execute.
    /// </summary>
    public string CommandId { get; }
    
    /// <summary>
    /// Gets the parsing result.
    /// </summary>
    public CommandArgumentResult Arguments { get; }
}