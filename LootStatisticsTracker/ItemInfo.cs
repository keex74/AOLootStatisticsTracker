// <copyright file="ItemInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using AOSharp.Core.Inventory;

/// <summary>
/// Defines information about an item.
/// </summary>
internal class ItemInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemInfo"/> class.
    /// </summary>
    public ItemInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemInfo"/> class.
    /// </summary>
    /// <param name="item">The source item.</param>
    public ItemInfo(Item item)
    {
        this.Name = item.Name;
        this.LowId = item.Id;
        this.HighId = item.HighId;
        this.QL = item.QualityLevel;
        this.Charges = item.Charges;
    }

    /// <summary>
    /// Gets or sets the item low ID.
    /// </summary>
    public int LowId { get; set; }

    /// <summary>
    /// Gets or sets the item high ID.
    /// </summary>
    public int HighId { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item QL.
    /// </summary>
    public int QL { get; set; }

    /// <summary>
    /// Gets or sets the number of charges.
    /// </summary>
    public int Charges { get; set; }
}
