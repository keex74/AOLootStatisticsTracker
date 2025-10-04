// <copyright file="BuffInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

/// <summary>
/// Defines information about a buff.
/// </summary>
internal class BuffInfo
{
    /// <summary>
    /// Gets or sets the buff item ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the buff name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timeleft value.
    /// </summary>
    public float TimeLeft { get; set; }
}
