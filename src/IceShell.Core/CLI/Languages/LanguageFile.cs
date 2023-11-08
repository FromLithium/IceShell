// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.CLI.Languages;

using Kajabity.Tools.Java;

/// <summary>
/// Represents a language file.
/// </summary>
public class LanguageFile
{
    /// <summary>
    /// Gets an empty language file.
    /// </summary>
    public static readonly LanguageFile Empty = new(new JavaProperties());

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageFile"/> class.
    /// </summary>
    /// <param name="properties">The properties file.</param>
    public LanguageFile(JavaProperties properties)
    {
        _properties = properties;
    }

    private readonly JavaProperties _properties;

    /// <summary>
    /// Gets the message associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the message to locate.</param>
    /// <returns>The localised message if found; otherwise the key.</returns>
    public string Get(string key)
    {
        if (!_properties.TryGetValue(key, out var value))
        {
            return key;
        }

        return value;
    }

    /// <summary>
    /// Gets the message associated with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="result">The message, if exists.</param>
    /// <returns><see langword="true"/> if this language file contains the specified key; otherwise <see langword="false"/>.</returns>
    public bool TryGet(string key, out string? result)
    {
        return _properties.TryGetValue(key, out result);
    }

    /// <summary>
    /// Determines whether this language file contains a message with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><see langword="true"/> if the message exists; otherwise, <see langword="false"/>.</returns>
    public bool Contains(string key)
    {
        return _properties.ContainsKey(key);
    }
}
