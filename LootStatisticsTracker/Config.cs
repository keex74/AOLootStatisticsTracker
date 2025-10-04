// <copyright file="Config.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

/// <summary>
/// Defines configuration data.
/// </summary>
public class Config
{
    /// <summary>
    /// Gets or sets the output path for files.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>
    /// Load the configuration data from the given location.
    /// </summary>
    /// <param name="configPath">The path to the config file.</param>
    /// <returns>The config data, or a default object if not loaded.</returns>
    public static Config Load(string configPath)
    {
        var data = File.ReadAllText(configPath);
        var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(data);
        settings ??= new Config();
        return settings;
    }

    /// <summary>
    /// Save the config to the given file.
    /// </summary>
    /// <param name="configPath">The path to save to.</param>
    public void Save(string configPath)
    {
        var txt = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        File.WriteAllText(configPath, txt);
    }
}
