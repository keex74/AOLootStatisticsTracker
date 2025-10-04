// <copyright file="MobStat.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

/// <summary>
/// Defines a stats value of a mob.
/// </summary>
internal class MobStat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MobStat"/> class.
    /// </summary>
    public MobStat()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MobStat"/> class.
    /// </summary>
    /// <param name="name">The stat name.</param>
    /// <param name="iD">The stat ID.</param>
    /// <param name="value">The stat value.</param>
    public MobStat(string name, int iD, int value)
    {
        this.Name = name;
        this.ID = iD;
        this.Value = value;
    }

    /// <summary>
    /// Gets or sets the stat name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stat ID.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the stat value.
    /// </summary>
    public int Value { get; set; }
}
