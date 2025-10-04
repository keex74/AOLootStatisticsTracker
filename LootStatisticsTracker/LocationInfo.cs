using AOSharp.Core;

namespace LootStatisticsTracker;

internal class LocationInfo
{
    public LocationInfo()
    {
    }

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

    public int PlayfieldID { get; set; }

    public string PlayfieldName { get; set; } = string.Empty;

    public bool IsDungeon { get; set; }

    public int Numfloors { get; set; }
}
