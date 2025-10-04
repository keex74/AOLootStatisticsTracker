// <copyright file="PositionInfo.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

using AOSharp.Common.GameData;

/// <summary>
/// Defines information about a position.
/// </summary>
internal class PositionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositionInfo"/> class.
    /// </summary>
    public PositionInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionInfo"/> class.
    /// </summary>
    /// <param name="vector">The source vector.</param>
    public PositionInfo(Vector3 vector)
    {
        this.X = vector.X;
        this.Y = vector.Y;
        this.Z = vector.Z;
    }

    /// <summary>
    /// Gets or sets the X postion.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the Y postion.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the Z postion.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Convert the position into a vector.
    /// </summary>
    /// <returns>The vector.</returns>
    public Vector3 ToVector()
    {
        return new Vector3(this.X, this.Y, this.Z);
    }
}
