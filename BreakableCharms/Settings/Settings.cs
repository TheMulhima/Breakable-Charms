using Osmi.Game;

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
    public Dictionary<int, CharmData> BrokenCharms = new()
    {
        { 1, new CharmData((int)Charm.GatheringSwarm) },
        { 2, new CharmData((int)Charm.WaywardCompass) },
        { 3, new CharmData((int)Charm.Grubsong) },
        { 4, new CharmData((int)Charm.StalwartShell) },
        { 5, new CharmData((int)Charm.BaldurShell) },
        { 6, new CharmData((int)Charm.FuryOfTheFallen) },
        { 7, new CharmData((int)Charm.QuickFocus) },
        { 8, new CharmData((int)Charm.LifebloodHeart) },
        { 9, new CharmData((int)Charm.LifebloodCore) },
        { 10, new CharmData((int)Charm.DefendersCrest) },
        { 11, new CharmData((int)Charm.Flukenest) },
        { 12, new CharmData((int)Charm.ThornsOfAgony) },
        { 13, new CharmData((int)Charm.MarkOfPride) },
        { 14, new CharmData((int)Charm.SteadyBody) },
        { 15, new CharmData((int)Charm.HeavyBlow) },
        { 16, new CharmData((int)Charm.SharpShadow) },
        { 17, new CharmData((int)Charm.SporeShroom) },
        { 18, new CharmData((int)Charm.Longnail) },
        { 19, new CharmData((int)Charm.ShamanStone) },
        { 20, new CharmData((int)Charm.SoulCatcher) },
        { 21, new CharmData((int)Charm.SoulEater) },
        { 22, new CharmData((int)Charm.GlowingWomb) },
        { 23, new HeartData() },
        { 24, new GreedData() },
        { 25, new StrengthData() },
        { 26, new CharmData((int)Charm.GatheringSwarm) },
        { 27, new CharmData((int)Charm.JonisBlessing) },
        { 28, new CharmData((int)Charm.ShapeOfUnn) },
        { 29, new CharmData((int)Charm.Hiveblood) },
        { 30, new CharmData((int)Charm.DreamWielder) },
        { 31, new CharmData((int)Charm.Dashmaster) },
        { 32, new CharmData((int)Charm.QuickSlash) },
        { 33, new CharmData((int)Charm.SpellTwister) },
        { 34, new CharmData((int)Charm.DeepFocus) },
        { 35, new CharmData((int)Charm.GrubberflysElegy) },
        { 36, new RoyalCharmData() },
        { 37, new CharmData((int)Charm.Sprintmaster) },
        { 38, new CharmData((int)Charm.Dreamshield) },
        { 39, new CharmData((int)Charm.Weaversong) },
        { 40, new GrimmChildData() },
    };
}

public sealed class ModGlobalSettings
{
    public bool RandomizeCharmLocations = false;
    public bool Break_DelicateCharms_On_AllDamage = false;
    public bool Break_DelicateCharms_On_DoubleDamage = true;
}

/// <summary>
/// only for interopping with rando
/// </summary>
public sealed class RandoSettings
{
    public bool RandomizeCharmLocations = false;
}