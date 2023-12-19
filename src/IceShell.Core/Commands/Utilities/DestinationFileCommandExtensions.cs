// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Utilities;

using IceShell.Core.Commands.Registry;

/// <summary>
/// Provides extension methods for destination file commands.
/// </summary>
public static class DestinationFileCommandExtensions
{
    /// <summary>
    /// Configures this <see cref="CommandBuilder"/> as a destination-based file operation command.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <returns>The command builder instance, for chaining.</returns>
    public static CommandBuilder ConfigureDestinationCommand(this CommandBuilder builder)
    {
        builder.SwitchOption('F')
            .CustomUsage("<sources...> <destination>")
            .VariableValues()
            .NoParseVariables();

        return builder;
    }
}