// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.CLI.Languages;

using System.Collections.ObjectModel;

/// <summary>
/// Represents a language file.
/// </summary>
public class LanguageFile
{
    /// <summary>
    /// Gets an empty language file.
    /// </summary>
    public static readonly LanguageFile Empty = new(new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()));

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageFile"/> class.
    /// </summary>
    /// <param name="properties">The properties file.</param>
    public LanguageFile(IReadOnlyDictionary<string, string> properties)
    {
        _properties = properties;
    }
    
    private readonly IReadOnlyDictionary<string, string> _properties;

    /// <summary>
    /// Gets the message associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the message to locate.</param>
    /// <returns>The localised message if found; otherwise the key.</returns>
    public string Get(string key)
    {
        return !_properties.TryGetValue(key, out var value) ? key : value;
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
