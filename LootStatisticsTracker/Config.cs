namespace LootStatisticsTracker;

public class Config
{
    /// <summary>
    /// Gets or sets the output path for files.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;

    public static Config Load(string configPath)
    {
        var data = File.ReadAllText(configPath);
        var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(data);
        settings ??= new Config();
        return settings;
    }

    public void Save(string configPath)
    {
        var txt = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        File.WriteAllText(configPath, txt);
    }
}
