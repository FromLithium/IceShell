// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.CLI.Languages;

using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using IceShell.Core.Settings;

/// <summary>
/// Provide methods and properties to help with the usage of language files.
/// </summary>
public class LanguageService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    /// <param name="shellSettings">The settings.</param>
    public LanguageService(ShellSettings shellSettings)
    {
        _shellSettings = shellSettings;
    }

    private ShellSettings _shellSettings;
    private readonly Dictionary<string, LanguageFile> _languageFiles = new();

    /// <summary>
    /// Gets the currently using language file.
    /// </summary>
    /// <returns></returns>
    public LanguageFile Current()
    {
        if (!_languageFiles.TryGetValue(_shellSettings.Language, out var value))
        {
            return LanguageFile.Empty;
        }

        return value;
    }

    /// <summary>
    /// Gets the <c>en</c> language file.
    /// </summary>
    /// <returns>The English language file.</returns>
    public LanguageFile English()
    {
        if (!_languageFiles.TryGetValue("en", out var value))
        {
            return LanguageFile.Empty;
        }

        return value;
    }

    /// <summary>
    /// Gets the current language service instance.
    /// </summary>
    public static LanguageService Instance { get; internal set; } = new(new());

    /// <summary>
    /// Sets the configuration used by the current language service instance to the specified configuration.
    /// </summary>
    /// <param name="settings">The configuration to use.</param>
    public static void SetInstanceConfig(ShellSettings settings)
    {
        Instance._shellSettings = settings;
    }

    /// <summary>
    /// Reloads all language files.
    /// </summary>
    public void Reload()
    {
        _languageFiles.Clear();
        Load();
    }

    /// <summary>
    /// Loads language files from the <c>lang</c> directory in the executing assembly directory.
    /// </summary>
    public void Load()
    {
        var targetPath = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "lang"));

        foreach (var file in targetPath.EnumerateFiles("*.json"))
        {
            Dictionary<string, string>? jsonFile;
            
            using (var stream = file.OpenRead())
            {
                jsonFile = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
            }

            if (jsonFile == null)
            {
                continue;
            }

            _languageFiles.Add(Path.GetFileNameWithoutExtension(file.Name), new LanguageFile(new ReadOnlyDictionary<string, string>(jsonFile)));
        }
    }
}
