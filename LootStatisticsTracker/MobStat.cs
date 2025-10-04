namespace LootStatisticsTracker;

internal class MobStat
{
    public MobStat()
    {
    }

    public MobStat(string name, int iD, int value)
    {
        Name = name;
        ID = iD;
        Value = value;
    }

    public string Name { get; set; } = string.Empty;

    public int ID { get; set; }

    public int Value { get; set; }
}
