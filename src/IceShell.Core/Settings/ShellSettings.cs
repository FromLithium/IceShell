// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Settings;

using IniParser;
using IniParser.Model;

/// <summary>
/// Configuration of a standard IceShell interpreter.
/// </summary>
public class ShellSettings
{
    private const string SectionLauncher = "Launcher";
    private const string SectionShell = "Shell";
    
    /// <summary>
    /// Gets or sets whether to display date and time when the shell interpreter starts.
    /// </summary>
    public bool DisplayDateTimeOnStartUp { get; set; }

    /// <summary>
    /// Gets or sets whether to display the version of the shell interpreter when it starts.
    /// </summary>
    public bool DisplayShellInfoOnStartUp { get; set; }

    /// <summary>
    /// Gets or sets whether the shell will attempt to terminate the current subprocess instead of
    /// killing it whenever possible.
    /// </summary>
    public bool TerminateUponInterrupt { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to expand variables for all value arguments.
    /// </summary>
    public bool ExpandVariablesForAllValues { get; set; }

    /// <summary>
    /// Gets or sets the language file to use.
    /// </summary>
    public string Language { get; set; } = "en";
    
    /// <summary>
    /// Gets or sets a value indicating whether the launcher should add platform modules.
    /// </summary>
    public bool AddPlatformModules { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the launcher should search for and add external modules on startup.
    /// </summary>
    public bool AddExternalModules { get; set; }

    /// <summary>
    /// Writes the configuration to the specified file. The existing contents of the file will be deleted.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    public void Save(string file)
    {
        var parser = new FileIniDataParser();

        var data = new IniData
        {
            [SectionShell] =
            {
                ["DisplayDateTimeOnStartup"] = DisplayDateTimeOnStartUp.ToString(),
                [nameof(DisplayShellInfoOnStartUp)] = DisplayShellInfoOnStartUp.ToString(),
                [nameof(Language)] = Language,
                [nameof(TerminateUponInterrupt)] = TerminateUponInterrupt.ToString(),
                [nameof(ExpandVariablesForAllValues)] = ExpandVariablesForAllValues.ToString()
            },
            [SectionLauncher] =
            {
                [nameof(AddPlatformModules)] = AddPlatformModules.ToString(),
                [nameof(AddExternalModules)] = AddExternalModules.ToString()
            }
        };

        parser.WriteFile(file, data);
    }

    /// <summary>
    /// Creates an instance of <see cref="ShellSettings"/> based on the contents of the specified file.
    /// </summary>
    /// <param name="file">The file to read from.</param>
    /// <returns>An instance of <see cref="ShellSettings"/> based on the contents of the specified file.</returns>
    public static ShellSettings LoadFromFile(string file)
    {
        var parser = new FileIniDataParser();

        var data = parser.ReadFile(file);

        return LoadFrom(data);
    }

    /// <summary>
    /// Creates an instance of <see cref="ShellSettings"/> with the specified <see cref="IniData"/>.
    /// </summary>
    /// <param name="data">The data to read from.</param>
    /// <returns>An instance of <see cref="ShellSettings"/> based on the specified <see cref="IniData"/>.</returns>
    public static ShellSettings LoadFrom(IniData data)
    {
        return new ShellSettings
        {
            // Add data here
            DisplayDateTimeOnStartUp = bool.Parse(GetSettingSafe(data, SectionShell, "DisplayDateTimeOnStartup", "false")),
            Language = GetSettingSafe(data, SectionShell, nameof(Language), "en"),
            DisplayShellInfoOnStartUp = bool.Parse(GetSettingSafe(data, SectionShell, nameof(DisplayShellInfoOnStartUp), "false")),
            AddPlatformModules = GetBoolSettingSafe(data, SectionLauncher, nameof(AddPlatformModules), true),
            TerminateUponInterrupt = GetBoolSettingSafe(data, SectionShell, nameof(TerminateUponInterrupt)),
            ExpandVariablesForAllValues = GetBoolSettingSafe(data, SectionShell, nameof(ExpandVariablesForAllValues), true),
            AddExternalModules = GetBoolSettingSafe(data, SectionLauncher, nameof(AddExternalModules), true)
        };
    }

    private static bool GetBoolSettingSafe(IniData data, string key, string name, bool defaultValue = false)
    {
        return !bool.TryParse(GetSettingSafe(data, key, name, string.Empty), out var result) 
            ? defaultValue : result;
    }
    
    private static string GetSettingSafe(IniData data, string key, string name, string defaultValue)
    {
        var section = data[key];

        if (!section.ContainsKey(name))
        {
            section[name] = defaultValue;
        }

        return section[name];
    }
}