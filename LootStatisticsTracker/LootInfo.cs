// <copyright file="LootInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;

/// <summary>
/// Gets or sets the main loot information object.
/// </summary>
internal class LootInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LootInfo"/> class.
    /// </summary>
    public LootInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LootInfo"/> class.
    /// </summary>
    /// <param name="corpse">The corpse object to take data from.</param>
    /// <param name="mobInfo">The buffered mob information.</param>
    public LootInfo(Corpse corpse, MobBufferInfo? mobInfo)
    {
        this.RecordedOnUnix = ToUnixTimeMilliseconds(DateTime.UtcNow);
        this.Instance = corpse.Identity.Instance;
        this.Name = corpse.Name;
        this.SourceName = mobInfo?.Name ?? string.Empty;
        this.Level = mobInfo?.Level ?? 0;
        this.Profession = mobInfo != null ? (int)mobInfo.Profession : 0;
        this.Breed = mobInfo != null ? (int)mobInfo.Breed : 0;
        this.Gender = mobInfo != null ? (int)mobInfo.Gender : 0;
        Chat.WriteLine($"Mob health: {(mobInfo != null ? mobInfo.MaxHealth : -1)}");
        this.Playfield = new LocationInfo(true);
        this.Position = new PositionInfo(corpse.Position);
        this.GlobalPosition = new PositionInfo(corpse.GlobalPosition);
        this.CorpseInstance = corpse.GetStat(Stat.CorpseInstance);

        foreach (var item in corpse.Container.Items)
        {
            this.Items.Add(new ItemInfo(item));
        }

        if (mobInfo != null)
        {
            foreach (var b in mobInfo.Buffs)
            {
                this.MobBuffs.Add(new BuffInfo() { Id = b.Id, Name = b.Name, TimeLeft = b.TimeLeft });
            }
        }

        if (mobInfo != null && mobInfo.Stats.Count > 0)
        {
            this.HadMobStats = true;
            foreach (var stat in mobInfo.Stats)
            {
                this.Stats.Add(stat);
            }
        }
        else
        {
            this.HadMobStats = false;
            foreach (Stat stat in Enum.GetValues(typeof(AOSharp.Common.GameData.Stat)))
            {
                var v = corpse.GetStat(stat);
                if (v != 1234567890)
                {
                    this.Stats.Add(new MobStat(stat.ToString(), (int)stat, v));
                }
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LootInfo"/> class.
    /// </summary>
    /// <param name="chest">The chest object to take data from.</param>
    public LootInfo(Chest chest)
    {
        this.RecordedOnUnix = ToUnixTimeMilliseconds(DateTime.UtcNow);
        this.Instance = chest.Identity.Instance;
        this.Name = chest.Name;
        this.Playfield = new LocationInfo(true);
        this.Position = new PositionInfo(chest.Position);
        this.GlobalPosition = new PositionInfo(chest.GlobalPosition);
        this.HadMobStats = false;
        foreach (Stat stat in Enum.GetValues(typeof(AOSharp.Common.GameData.Stat)))
        {
            var v = chest.GetStat(stat);
            if (v != 1234567890)
            {
                this.Stats.Add(new MobStat(stat.ToString(), (int)stat, v));
            }
        }

        foreach (var item in chest.Container.Items)
        {
            this.Items.Add(new ItemInfo(item));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the data had mob statistics.
    /// </summary>
    public bool HadMobStats { get; set; }

    /// <summary>
    /// Gets or sets the unix timestamp of the data object creation.
    /// </summary>
    public long RecordedOnUnix { get; set; }

    /// <summary>
    /// Gets or sets the instance ID of the corpse or chest.
    /// </summary>
    public int Instance { get; set; }

    /// <summary>
    /// Gets or sets the name of the corpse or chest.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the mob associated with the corpse, if available.
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CorpseInstance value, defining the instance value of the source dynel.
    /// </summary>
    public int CorpseInstance { get; set; }

    /// <summary>
    /// Gets or sets the level of the source mob.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the profession of the source mob.
    /// </summary>
    public int Profession { get; set; }

    /// <summary>
    /// Gets or sets the breed of the source mob.
    /// </summary>
    public int Breed { get; set; }

    /// <summary>
    /// Gets or sets the gender of the source mob.
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// Gets or sets the playfield information of the corpse location.
    /// </summary>
    public LocationInfo Playfield { get; set; } = new();

    /// <summary>
    /// Gets or sets the position of the corpse.
    /// </summary>
    public PositionInfo Position { get; set; } = new();

    /// <summary>
    /// Gets or sets the global position of the corpse.
    /// </summary>
    public PositionInfo GlobalPosition { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of stats of the mob, if available.
    /// </summary>
    public List<MobStat> Stats { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of items in the corpse.
    /// </summary>
    public List<ItemInfo> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of buffs on the source mob, if available.
    /// </summary>
    public List<BuffInfo> MobBuffs { get; set; } = [];

    /// <summary>
    /// Convert a date value into a unix timestamp.
    /// </summary>
    /// <param name="dateTime">The date.</param>
    /// <returns>The millisecond unix timestamp.</returns>
    public static long ToUnixTimeMilliseconds(DateTime dateTime)
    {
        DateTimeOffset offset = (DateTimeOffset)dateTime.ToUniversalTime();
        return offset.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Convert a unix timestamp into a date value.
    /// </summary>
    /// <param name="unixTimeMilliseconds">The unix timestamp in milliseconds.</param>
    /// <returns>The date value.</returns>
    public static DateTime FromUnixTimeMilliseconds(long unixTimeMilliseconds)
    {
        var off = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
        return off.UtcDateTime;
    }
}
