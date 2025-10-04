using AOSharp.Common.GameData;
using AOSharp.Core;

namespace LootStatisticsTracker;

internal class LootInfo
{
    public LootInfo()
    {
    }

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
        this.Playfield = new LocationInfo(true);
        this.Position = new PositionInfo(corpse.Position);
        this.GlobalPosition = new PositionInfo(corpse.GlobalPosition);
        this.CorpseInstance = corpse.GetStat(Stat.CorpseInstance);
        foreach (Stat stat in Enum.GetValues(typeof(AOSharp.Common.GameData.Stat)))
        {
            var v = corpse.GetStat(stat);
            if (v != 1234567890)
            {
                this.Stats.Add(new MobStat(stat.ToString(), (int)stat, v));
            }
        }

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
    }

    public LootInfo(Chest chest)
    {
        this.RecordedOnUnix = ToUnixTimeMilliseconds(DateTime.UtcNow);
        this.Instance = chest.Identity.Instance;
        this.Name = chest.Name;
        this.Playfield = new LocationInfo(true);
        this.Position = new PositionInfo(chest.Position);
        this.GlobalPosition = new PositionInfo(chest.GlobalPosition);
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

    public long RecordedOnUnix { get; set; }

    public int Instance { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;

    public int CorpseInstance { get; set; }

    public int Level { get; set; }

    public int Profession { get; set; }

    public int Breed { get; set; }

    public int Gender { get; set; }

    public LocationInfo Playfield { get; set; } = new();

    public PositionInfo Position { get; set; } = new();

    public PositionInfo GlobalPosition { get; set; } = new();

    public List<MobStat> Stats { get; set; } = [];

    public List<ItemInfo> Items { get; set; } = [];

    public List<BuffInfo> MobBuffs { get; set; } = [];

    private static long ToUnixTimeMilliseconds(DateTime dateTime)
    {
        DateTimeOffset offset = (DateTimeOffset)dateTime.ToUniversalTime();
        return offset.ToUnixTimeMilliseconds();
    }

    public static DateTime FromUnixTimeMilliseconds(long unixTimeMilliseconds)
    {
        var off = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
        return off.UtcDateTime;
    }
}
