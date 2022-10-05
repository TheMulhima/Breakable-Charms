namespace BreakableCharms;

public class LocalSettings
{
    public LocalSettings()
    {
        foreach (var (charmNum, charmData) in BrokenCharms)
        {
            if (charmData.HasSpecialSprite) continue;
            Dictionaries.BrokenCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Images.Broken.Charm_{charmNum}");
            Dictionaries.DelicateCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Images.Delicate.Charm_{charmNum}");
            Dictionaries.FragileCharmSpriteFromID[charmNum] = SpriteUtils.LoadSpriteFromResources($"Images.Fragile.Charm_{charmNum}");
        }
    }
    //charmNum, (CharmState, IsBroken)
    public Dictionary<int, CharmData> BrokenCharms = new()
    {
        { 1, new CharmData(1) },
        { 2, new CharmData(2) },
        { 3, new CharmData(3) },
        { 4, new CharmData(4) },
        { 5, new CharmData(5) },
        { 6, new CharmData(6) },
        { 7, new CharmData(7) },
        { 8, new CharmData(8) },
        { 9, new CharmData(9) },
        { 10, new CharmData(10) },
        { 11, new CharmData(11) },
        { 12, new CharmData(12) },
        { 13, new CharmData(13) },
        { 14, new CharmData(14) },
        { 15, new CharmData(15) },
        { 16, new CharmData(16) },
        { 17, new CharmData(17) },
        { 18, new CharmData(18) },
        { 19, new CharmData(19) },
        { 20, new CharmData(20) },
        { 21, new CharmData(21) },
        { 22, new CharmData(22) },
        { 23, new HeartData() },
        { 24, new GreedData() },
        { 25, new StrengthData() },
        { 26, new CharmData(26) },
        { 27, new CharmData(27) },
        { 28, new CharmData(28) },
        { 29, new CharmData(29) },
        { 30, new CharmData(30) },
        { 31, new CharmData(31) },
        { 32, new CharmData(32) },
        { 33, new CharmData(33) },
        { 34, new CharmData(34) },
        { 35, new CharmData(35) },
        { 36, new RoyalCharmData() },
        { 37, new CharmData(37) },
        { 38, new CharmData(38) },
        { 39, new CharmData(39) },
        { 40, new GrimmChildData() },
    };
}

public class GlobalSettings
{
    public bool BreakOnAllDamage = false;
    public bool BreakOnDoubleDamage = true;
}