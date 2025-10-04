using AOSharp.Common.GameData;
using AOSharp.Core;

namespace LootStatisticsTracker;

internal class MobBufferInfo
{
    public MobBufferInfo(Dynel dynel, bool recordStats)
    {
        if (dynel.Identity.Type == IdentityType.SimpleChar)
        {
            var sc = new SimpleChar(dynel);
            this.Level = sc.Level;
            this.Profession = sc.Profession;
            this.Breed = sc.Breed;
            this.Name = sc.Name;
            this.MaxHealth = sc.MaxHealth;
            this.Gender = (Gender)sc.GetStat(Stat.Sex);
            foreach (var buff in sc.Buffs)
            {
                var b = new BuffInfo()
                {
                    Id = buff.Id,
                    Name = buff.Name,
                    TimeLeft = buff.RemainingTime,
                };

                this.Buffs.Add(b);
            }

            if (recordStats)
            {
                foreach (AOSharp.Common.GameData.Stat stat in Enum.GetValues(typeof(AOSharp.Common.GameData.Stat)))
                {
                    var v = dynel.GetStat(stat);
                    if (v != 1234567890)
                    {
                        this.Stats.Add(new MobStat(stat.ToString(), (int)stat, v));
                    }
                }
            }
        }
        else
        {
            this.Name = dynel.Name;
        }
    }

    public int Level { get; set; }

    public Profession Profession { get; set; }

    public Breed Breed { get; set; }

    public Gender Gender { get; set; }

    public string Name { get; set; }

    public int MaxHealth { get; set; }

    public List<BuffInfo> Buffs { get; set; } = [];

    public List<MobStat> Stats { get; set; } = [];
}
