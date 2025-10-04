using AOSharp.Core.Inventory;

namespace LootStatisticsTracker;

internal class ItemInfo
{
    public ItemInfo()
    {
    }

    public ItemInfo(Item item)
    {
        this.Name = item.Name;
        this.LowId = item.Id;
        this.HighId = item.HighId;
        this.QL = item.QualityLevel;
        this.Charges = item.Charges;
    }

    public int LowId { get; set; }

    public int HighId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int QL { get; set; }

    public int Charges { get; set; }
}
