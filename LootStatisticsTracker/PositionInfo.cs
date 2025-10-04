using AOSharp.Common.GameData;

namespace LootStatisticsTracker;

internal class PositionInfo
{
    public PositionInfo()
    {
    }

    public PositionInfo(Vector3 vector)
    {
        this.X = vector.X;
        this.Y = vector.Y;
        this.Z = vector.Z;
    }

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public Vector3 ToVector()
    {
        return new Vector3(this.X, this.Y, this.Z);
    }
}
