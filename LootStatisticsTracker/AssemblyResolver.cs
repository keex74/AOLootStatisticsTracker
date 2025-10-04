// <copyright file="AssemblyResolver.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using System.Reflection;
using AOSharp.Core.UI;

/// <summary>
/// Defines a class that implements lazy loading of assembly references.
/// </summary>
internal class AssemblyResolver
{
    /// <summary>
    /// Gets or sets the plugin directory.
    /// </summary>
    public string PluginDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Resolve the given assembly.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    /// <returns>The loaded assembly, if any.</returns>
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
