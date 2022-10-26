﻿using Osmi.Game;

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
        { 1, new CharmData.CharmData((int)Charm.GatheringSwarm) },
        { 2, new CharmData.CharmData((int)Charm.WaywardCompass) },
        { 3, new CharmData.CharmData((int)Charm.Grubsong) },
        { 4, new CharmData.CharmData((int)Charm.StalwartShell) },
        { 5, new CharmData.CharmData((int)Charm.BaldurShell) },
        { 6, new CharmData.CharmData((int)Charm.FuryOfTheFallen) },
        { 7, new CharmData.CharmData((int)Charm.QuickFocus) },
        { 8, new CharmData.CharmData((int)Charm.LifebloodHeart) },
        { 9, new CharmData.CharmData((int)Charm.LifebloodCore) },
        { 10, new CharmData.CharmData((int)Charm.DefendersCrest) },
        { 11, new CharmData.CharmData((int)Charm.Flukenest) },
        { 12, new CharmData.CharmData((int)Charm.ThornsOfAgony) },
        { 13, new CharmData.CharmData((int)Charm.MarkOfPride) },
        { 14, new CharmData.CharmData((int)Charm.SteadyBody) },
        { 15, new CharmData.CharmData((int)Charm.HeavyBlow) },
        { 16, new CharmData.CharmData((int)Charm.SharpShadow) },
        { 17, new CharmData.CharmData((int)Charm.SporeShroom) },
        { 18, new CharmData.CharmData((int)Charm.Longnail) },
        { 19, new CharmData.CharmData((int)Charm.ShamanStone) },
        { 20, new CharmData.CharmData((int)Charm.SoulCatcher) },
        { 21, new CharmData.CharmData((int)Charm.SoulEater) },
        { 22, new CharmData.CharmData((int)Charm.GlowingWomb) },
        { 23, new HeartData() },
        { 24, new GreedData() },
        { 25, new StrengthData() },
        { 26, new CharmData.CharmData((int)Charm.GatheringSwarm) },
        { 27, new CharmData.CharmData((int)Charm.JonisBlessing) },
        { 28, new CharmData.CharmData((int)Charm.ShapeOfUnn) },
        { 29, new CharmData.CharmData((int)Charm.Hiveblood) },
        { 30, new CharmData.CharmData((int)Charm.DreamWielder) },
        { 31, new CharmData.CharmData((int)Charm.Dashmaster) },
        { 32, new CharmData.CharmData((int)Charm.QuickSlash) },
        { 33, new CharmData.CharmData((int)Charm.SpellTwister) },
        { 34, new CharmData.CharmData((int)Charm.DeepFocus) },
        { 35, new CharmData.CharmData((int)Charm.GrubberflysElegy) },
        { 36, new RoyalCharmData() },
        { 37, new CharmData.CharmData((int)Charm.Sprintmaster) },
        { 38, new CharmData.CharmData((int)Charm.Dreamshield) },
        { 39, new CharmData.CharmData((int)Charm.Weaversong) },
        { 40, new GrimmChildData() },
    };
}

public sealed class GlobalSettings
{
    public bool RandomizeCharmLocations = false;
    public bool Break_DelicateCharms_On_AllDamage = false;
    public bool Break_DelicateCharms_On_DoubleDamage = true;
}