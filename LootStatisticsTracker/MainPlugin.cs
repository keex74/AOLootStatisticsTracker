using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace LootStatisticsTracker;

public class MainPlugin
    : AOPluginEntry
{
    private readonly Dictionary<long, MobBufferInfo> MobInfoBuffer = [];
    private readonly SqliteWriter sqliteWriter = new();
    private readonly AssemblyResolver assemblyResolver = new();
    private string configPath = string.Empty;
    private Config settings = new();

    public MainPlugin()
    {
    }

    public override void Run()
    {
        this.assemblyResolver.PluginDirectory = this.PluginDirectory;
        AppDomain.CurrentDomain.AssemblyResolve -= this.assemblyResolver.ResolveAssembly;
        AppDomain.CurrentDomain.AssemblyResolve += this.assemblyResolver.ResolveAssembly;

        this.configPath = Path.Combine(this.PluginDirectory, "config.json");
        if (File.Exists(configPath))
        {
            this.settings = Config.Load(this.configPath);
        }
        else
        {
            this.settings = new();
        }

        Network.N3MessageReceived += this.HandleN3MessageReceived;
        DynelManager.DynelSpawned += this.HandleDynelSpawned;
        Inventory.ContainerOpened += this.HandleContainerOpened;

        if (string.IsNullOrEmpty(this.settings.OutputPath))
        {
            Chat.WriteLine("Output database not specified.", ChatColor.Green);
            Chat.WriteLine("Do /lst setoutput \"<path+filename>\"", ChatColor.Green);
        }

        Chat.RegisterCommand("lst", HandleLstCallback);
        this.RefreshPath();
        this.InitializeMobBuffer();
        Chat.WriteLine("LootStatisticsTracker loaded.");
        Chat.WriteLine("Set output database: /lst setoutput \"<path+filename>\"");
    }

    public override void Teardown()
    {
        base.Teardown();
        this.sqliteWriter.SetPath(string.Empty);
        AppDomain.CurrentDomain.AssemblyResolve -= this.assemblyResolver.ResolveAssembly;
    }

    private void HandleLstCallback(string command, string[] args, ChatWindow chat)
    {
        if (args.Length == 0)
        {
            chat.WriteLine("No sub command specified.", ChatColor.Orange);
            return;
        }

        if (args[0] == "setoutput" && args.Length < 2)
        {
            chat.WriteLine("Specify an output path as the second argument.", ChatColor.Orange);
            return;
        }
        else if (args[0] == "setoutput")
        {
            if (string.IsNullOrEmpty(this.configPath))
            {
                chat.WriteLine("No config file specified.", ChatColor.Red);
                return;
            }

            var path = args[1];
            if (path.StartsWith("\""))
            {
                path = path.Substring(1);
            }

            if (path.EndsWith("\""))
            {
                path = path.Substring(0, path.Length - 1);
            }

            this.settings.OutputPath = path;
            this.settings.Save(this.configPath);
            this.RefreshPath();
            chat.WriteLine($"Output path set to: {path}");
            return;
        }
    }

    private void RefreshPath()
    {
        if (string.IsNullOrEmpty(this.settings.OutputPath))
        {
            this.sqliteWriter.SetPath(string.Empty);
            return;
        }

        this.sqliteWriter.SetPath(this.settings.OutputPath);
    }

    private void InitializeMobBuffer()
    {
        this.MobInfoBuffer.Clear();
        foreach (var dynel in DynelManager.AllDynels)
        {
            if (dynel.Identity.Type == AOSharp.Common.GameData.IdentityType.SimpleChar)
            {
                var sc = new SimpleChar(dynel);
                this.MobInfoBuffer[dynel.Identity.Instance] = new MobBufferInfo(sc, false);
            }
        }
    }

    private void HandleN3MessageReceived(object sender, N3Message msg)
    {
        // Update a mob buffer dynel when it is attacked or otherwise fully updated,
        // otherwise e.g. the profession does not show up.
        // In this case, update all dynel stats as well
        if (msg is AttackMessage attackMsg)
        {
            if (attackMsg.Target.Type == IdentityType.SimpleChar)
            {
                if (DynelManager.Find(attackMsg.Target, out Dynel dynel))
                {
                    var sc = new SimpleChar(dynel);
                    this.MobInfoBuffer[dynel.Identity.Instance] = new MobBufferInfo(sc, true);
                }
            }
        }
        else if (msg is SmokeLounge.AOtomation.Messaging.Messages.N3Messages.InfoPacketMessage infomsg)
        {
            if (infomsg.Identity.Type == IdentityType.SimpleChar)
            {
                if (DynelManager.Find(infomsg.Identity, out Dynel dynel))
                {
                    var sc = new SimpleChar(dynel);
                    this.MobInfoBuffer[dynel.Identity.Instance] = new MobBufferInfo(sc, true);
                }
            }
        }
    }

    private void HandleDynelSpawned(object? sender, Dynel dynel)
    {
        // Add dynel to mob buffer without all stats
        if (dynel.Identity.Type == AOSharp.Common.GameData.IdentityType.SimpleChar)
        {
            var sc = new SimpleChar(dynel);
            this.MobInfoBuffer[dynel.Identity.Instance] = new MobBufferInfo(sc, false);
        }
    }

    private void HandleContainerOpened(object? sender, AOSharp.Core.Inventory.Container container)
    {
        if (container == null)
        {
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(this.settings.OutputPath))
            {
                Chat.WriteLine("No output path specified. Discarding info.", ChatColor.Orange);
                return;
            }
            else if (!Directory.Exists(Path.GetDirectoryName(this.settings.OutputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.settings.OutputPath));
            }
        }
        catch (Exception ex)
        {
            Chat.WriteLine($"Error accessing output directory: {ex.Message}", ChatColor.Red);
            return;
        }

        if (container.Identity.Type == IdentityType.Corpse)
        {
            var sourceCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Identity == container.Identity);
            if (sourceCorpse == null)
            {
                return;
            }

            Chat.WriteLine($"Corpse opened: {container.Identity.Instance} - {sourceCorpse.Name}");
            var contents = GetLootInfo(sourceCorpse);
            try
            {
                var inserted = this.sqliteWriter.InsertCorpse(contents, false);
                if (!inserted)
                {
                    Chat.WriteLine($"Failed to insert corpse into database for some reason :(", ChatColor.Red);
                }
            }
            catch (Exception ex)
            {
                Chat.WriteLine($"DB Error: {ex}", ChatColor.Red);
            }
        }
        else if (container.Identity.Type == IdentityType.Container)
        {
            var sourceChest = DynelManager.Chests.FirstOrDefault(c => c.Identity == container.Identity);
            if (sourceChest == null)
            {
                return;
            }

            Chat.WriteLine($"Chest opened: {container.Identity.Instance} - {sourceChest.Name}");
            var contents = GetLootInfo(sourceChest);
            try
            {
                var inserted = this.sqliteWriter.InsertContainer(contents);
                if (!inserted)
                {
                    Chat.WriteLine($"Failed to insert container into database for some reason :(", ChatColor.Red);
                }
            }
            catch (Exception ex)
            {
                Chat.WriteLine($"DB Error: {ex}", ChatColor.Red);
            }
        }
    }

    private LootInfo GetLootInfo(Corpse corpse)
    {
        var ident = corpse.GetStat(AOSharp.Common.GameData.Stat.CorpseInstance);
        if (this.MobInfoBuffer.TryGetValue(ident, out var mobInfo))
        {
            return new LootInfo(corpse, mobInfo);
        }
        else
        {
            return new LootInfo(corpse, null);
        }
    }

    private LootInfo GetLootInfo(Chest chest)
    {
        return new LootInfo(chest);
    }
}
