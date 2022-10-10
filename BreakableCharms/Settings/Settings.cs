namespace BreakableCharms.Settings;

public sealed class LocalSettings
{
    public LocalSettings()
    {
        foreach (var (charmNum, CharmData) in BrokenCharms)
        {
            if (CharmData.HasSpecialSprite) continue;
            Dictionaries.BrokenCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Broken.Charm_{charmNum}");
            Dictionaries.DelicateCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Delicate.Charm_{charmNum}");
            Dictionaries.FragileCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Fragile.Charm_{charmNum}");
        }
    }
    //charmNum, (CharmState, IsBroken)
    public Dictionary<int, CharmData.CharmData> BrokenCharms = new()
    {
        { 1, new CharmData.CharmData(1) },
        { 2, new CharmData.CharmData(2) },
        { 3, new CharmData.CharmData(3) },
        { 4, new CharmData.CharmData(4) },
        { 5, new CharmData.CharmData(5) },
        { 6, new CharmData.CharmData(6) },
        { 7, new CharmData.CharmData(7) },
        { 8, new CharmData.CharmData(8) },
        { 9, new CharmData.CharmData(9) },
        { 10, new CharmData.CharmData(10) },
        { 11, new CharmData.CharmData(11) },
        { 12, new CharmData.CharmData(12) },
        { 13, new CharmData.CharmData(13) },
        { 14, new CharmData.CharmData(14) },
        { 15, new CharmData.CharmData(15) },
        { 16, new CharmData.CharmData(16) },
        { 17, new CharmData.CharmData(17) },
        { 18, new CharmData.CharmData(18) },
        { 19, new CharmData.CharmData(19) },
        { 20, new CharmData.CharmData(20) },
        { 21, new CharmData.CharmData(21) },
        { 22, new CharmData.CharmData(22) },
        { 23, new HeartData() },
        { 24, new GreedData() },
        { 25, new StrengthData() },
        { 26, new CharmData.CharmData(26) },
        { 27, new CharmData.CharmData(27) },
        { 28, new CharmData.CharmData(28) },
        { 29, new CharmData.CharmData(29) },
        { 30, new CharmData.CharmData(30) },
        { 31, new CharmData.CharmData(31) },
        { 32, new CharmData.CharmData(32) },
        { 33, new CharmData.CharmData(33) },
        { 34, new CharmData.CharmData(34) },
        { 35, new CharmData.CharmData(35) },
        { 36, new RoyalCharmData() },
        { 37, new CharmData.CharmData(37) },
        { 38, new CharmData.CharmData(38) },
        { 39, new CharmData.CharmData(39) },
        { 40, new GrimmChildData() },
    };
}

public sealed class GlobalSettings
{
    public bool RandomizeCharmLocations = false;
    public bool Break_DelicateCharms_On_AllDamage = false;
    public bool Break_DelicateCharms_On_DoubleDamage = true;
}