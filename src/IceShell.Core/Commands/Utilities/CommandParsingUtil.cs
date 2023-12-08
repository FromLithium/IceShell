// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Utilities;

using System.Text;
using IceShell.Core.Api;
using IceShell.Core.Exceptions;

/// <summary>
/// Provides methods to aid in command parsing.
/// </summary>
public class CommandParsingUtil
{
    /// <summary>
    /// Evaluate a path and expands all context variables within.
    /// </summary>
    /// <param name="from">The path to expand.</param>
    /// <param name="document">The document where variables are stored in.</param>
    /// <param name="lenient">If <see langword="true"/>, certain syntax wrongs in the original string will be ignored.</param>
    /// <returns>The expanded path.</returns>
    /// <exception cref="ArgumentException">The specified <paramref name="document"/> have its <see cref="ICommandDocument.Variables"/> property set to <see langword="null"/>.</exception>
    public static string ExpandContextVariables(ReadOnlySpan<char> from, ICommandDocument document, bool lenient = false)
    {
        if (document.Variables == null)
        {
            throw new ArgumentException("The specified document has no variable support.", nameof(document));
        }
        
        var inVariable = false;
        
        var builder = new StringBuilder();
        var varBuilder = new StringBuilder();

        foreach (var x in from)
        {
            if (x == '$')
            {
                if (!inVariable)
                {
                    // Begin an environment variable.
                    inVariable = true;
                    continue;
                }

                // End an environment variable.
                inVariable = false;

                // Get variable name and clear builder to reuse.
                var variableName = varBuilder.ToString();
                varBuilder.Clear();

                if (!string.IsNullOrEmpty(variableName))
                {
                    if (!document.Variables!.TryGetValue(variableName, out var contextVar))
                    {
                        throw ExceptionHelper.UnknownEnvironmentVariable(variableName);
                    }
                    
                    builder.Append(contextVar);
                    continue;
                }

                // If otherwise, this is hereby escaped.
            }

            if (inVariable)
            {
                varBuilder.Append(x);
            }
            else
            {
                builder.Append(x);
            }
        }

        if (inVariable && !lenient)
        {
            throw ExceptionHelper.WithMessage("generic_env_variable_never_end");
        }

        return builder.ToString();
    }
}