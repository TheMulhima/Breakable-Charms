namespace BreakableCharms;

public enum CharmState
{
    Fragile = 0,
    Durable,
    Unbreakable,
}

public class CharmData
{
    public bool isBroken;
    public CharmState charmState;

    public CharmData()
    {
        isBroken = false;
        charmState = CharmState.Fragile;
    }

}

public class LocalSettings
{
    //charmNum, (CharmState, IsBroken)
    public Dictionary<int, CharmData> BrokenCharms = new()
    {
        { 1, new CharmData() },
        { 2, new CharmData() },
        { 3, new CharmData() },
        { 4, new CharmData() },
        { 5, new CharmData() },
        { 6, new CharmData() },
        { 7, new CharmData() },
        { 8, new CharmData() },
        { 9, new CharmData() },
        { 10, new CharmData() },
        { 11, new CharmData() },
        { 12, new CharmData() },
        { 13, new CharmData() },
        { 14, new CharmData() },
        { 15, new CharmData() },
        { 16, new CharmData() },
        { 17, new CharmData() },
        { 18, new CharmData() },
        { 19, new CharmData() },
        { 20, new CharmData() },
        { 21, new CharmData() },
        { 22, new CharmData() },
        //skip the already breakable charms
        { 26, new CharmData() },
        { 27, new CharmData() },
        { 28, new CharmData() },
        { 29, new CharmData() },
        { 30, new CharmData() },
        { 31, new CharmData() },
        { 32, new CharmData() },
        { 33, new CharmData() },
        { 34, new CharmData() },
        { 35, new CharmData() },
        //skip voidheat
        { 37, new CharmData() },
        { 38, new CharmData() },
        { 39, new CharmData() },
        { 40, new CharmData() },
    };
}

public class GlobalSettings
{
    public bool BreakOnAllDamage = false;
    public bool BreakOnDoubleDamage = true;
}