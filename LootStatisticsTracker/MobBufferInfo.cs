// <copyright file="MobBufferInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using AOSharp.Common.GameData;
using AOSharp.Core;

/// <summary>
/// Defines buffered information about a mob.
/// </summary>
internal class MobBufferInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MobBufferInfo"/> class.
    /// </summary>
    /// <param name="dynel">The source dynel.</param>
    /// <param name="recordStats">Whether to record all stats values.</param>
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

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the profession.
    /// </summary>
    public Profession Profession { get; set; }

    /// <summary>
    /// Gets or sets the breed.
    /// </summary>
    public Breed Breed { get; set; }

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the maximum health.
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public List<BuffInfo> Buffs { get; set; } = [];

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public List<MobStat> Stats { get; set; } = [];
}
