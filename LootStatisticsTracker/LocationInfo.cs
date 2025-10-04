// <copyright file="LocationInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using AOSharp.Core;

/// <summary>
/// Defines information about a location.
/// </summary>
internal class LocationInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationInfo"/> class.
    /// </summary>
    public LocationInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationInfo"/> class.
    /// </summary>
    /// <param name="populate">If true, the object is initialized from the current playfield data.</param>
    public LocationInfo(bool populate)
    {
        if (populate)
        {
            this.PlayfieldID = (int)Playfield.ModelId;
            this.PlayfieldName = $"{Playfield.Name} - {Playfield.ModelId}";
            this.IsDungeon = Playfield.IsDungeon;
            this.Numfloors = Playfield.NumFloors;
        }
    }

    /// <summary>
    /// Gets or sets the playfield ID.
    /// </summary>
    public int PlayfieldID { get; set; }

    /// <summary>
    /// Gets or sets the playfield name.
    /// </summary>
    public string PlayfieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the playfield was a dungeon.
    /// </summary>
    public bool IsDungeon { get; set; }

    /// <summary>
    /// Gets or sets the number of floor of the dungeon.
    /// </summary>
    public int Numfloors { get; set; }
}
