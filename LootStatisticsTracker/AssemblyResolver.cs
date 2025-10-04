using AOSharp.Core.UI;
using System.Reflection;

namespace LootStatisticsTracker;

internal class AssemblyResolver
{
    public string PluginDirectory { get; set; } = string.Empty;

    public System.Reflection.Assembly? ResolveAssembly(object sender, ResolveEventArgs args)
    {
        var pluginDir = !string.IsNullOrEmpty(this.PluginDirectory) ? this.PluginDirectory : Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

        try
        {
            if (args.Name.Contains(".resources"))
            {
                return null;
            }

            var name = new AssemblyName(args.Name);
            Chat.WriteLine($"Try load assembly: {name}");
            var filename = name.Name + ".dll";
            var path = Path.Combine(pluginDir, filename);
            if (File.Exists(path))
            {
                try
                {
                    var ass = Assembly.LoadFile(path);
                    Chat.WriteLine($"Loaded assembly: {name}");
                    return ass;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Chat.WriteLine($"Assembly load error: {ex}");
            return null;
        }
    }
}
