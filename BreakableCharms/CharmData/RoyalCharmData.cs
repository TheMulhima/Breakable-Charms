namespace BreakableCharms.CharmData;
public sealed class RoyalCharmData : CharmData
{
    public RoyalCharmData():base(36) {}
    public override bool HasSpecialSprite => true;

    private static int RoyalCharmState
    {
        get
        {
            //i dont trust people to set royal charm state
            if (PlayerData.instance.GetBool(nameof(PlayerData.gotShadeCharm))) return 4;
            
            return PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState));
        }
    }

    public static Dictionary<int, Sprite> BrokenSprites = new Dictionary<int, Sprite>()
    {
        {1, SpriteUtils.LoadSpriteFromResources("Broken.Special.Charm_36_1")},
        {2, SpriteUtils.LoadSpriteFromResources("Broken.Special.Charm_36_2")},
        {3, SpriteUtils.LoadSpriteFromResources("Broken.Special.Charm_36_3")},
        {4, SpriteUtils.LoadSpriteFromResources("Broken.Special.Charm_36_4")},
    }; 
    public static Dictionary<int, Sprite> FragileSprites = new Dictionary<int, Sprite>()
    {
        {1, SpriteUtils.LoadSpriteFromResources("Fragile.Special.Charm_36_1")},
        {2, SpriteUtils.LoadSpriteFromResources("Fragile.Special.Charm_36_2")},
        {3, SpriteUtils.LoadSpriteFromResources("Fragile.Special.Charm_36_3")},
        {4, SpriteUtils.LoadSpriteFromResources("Fragile.Special.Charm_36_4")},
    }; 
    public static Dictionary<int, Sprite> DelicateSprites = new Dictionary<int, Sprite>()
    {
        {1, SpriteUtils.LoadSpriteFromResources("Delicate.Special.Charm_36_1")},
        {2, SpriteUtils.LoadSpriteFromResources("Delicate.Special.Charm_36_2")},
        {3, SpriteUtils.LoadSpriteFromResources("Delicate.Special.Charm_36_3")},
        {4, SpriteUtils.LoadSpriteFromResources("Delicate.Special.Charm_36_4")},
    }; 
        
    public override Sprite GetSprite()
    {
        if (RoyalCharmState == 0) return DelicateSprites[3];
        
        if (isBroken)
        {
            return BrokenSprites[RoyalCharmState];
        }

        if (charmState is CharmState.Delicate or CharmState.UnObtained)
        {
            return DelicateSprites[RoyalCharmState];
        }
        if (charmState is CharmState.Fragile)
        {
            return FragileSprites[RoyalCharmState];
        }
        if (charmState is CharmState.Unbreakable)
        {
            return RoyalCharmState switch
            {
                1 => BreakableCharms.queensFragment,
                2 => BreakableCharms.kingsFragment,
                3 => BreakableCharms.kingSoul,
                4 => Dictionaries.UnbreakableCharmSpriteFromID[charmNum],
                _ => BreakableCharms.kingSoul,
            };
        }
        return null;
    }
    private string AddSuffix(string key)
    {
        if (key.EndsWith("_A") || key.EndsWith("_B") || key.EndsWith("_C")) return key;
        return key + RoyalCharmState switch
        {
            1 => "_A",
            2 => "_A",
            3 => "_B",
            4 => "_C",
            _ => "_A"
        };
    }
    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        orig = Language.Language.GetInternal(AddSuffix(key), sheetitle);
        if (isBroken) return "Broken " + orig;
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                if (RoyalCharmState == 0) return "Delicate Royal Charm";
                return "Delicate " + orig;
            case CharmState.Fragile:
                if (RoyalCharmState == 0) return "Fragile Royal Charm";
                return "Fragile " + orig;
            case CharmState.Unbreakable:
                if (RoyalCharmState == 0) return "Unbreakable Royal Charm";
                return "Unbreakable " + orig;
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        orig = Language.Language.GetInternal(AddSuffix(key), sheetitle);
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                if (RoyalCharmState == 0) return "A delicate shell for the royal charm." + Consts.DelicateCharmDesc;
                return "A delicate charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
            case CharmState.Fragile:
                if (RoyalCharmState == 0) return "A fragile shell for the royal charm." + Consts.DelicateCharmDesc;
                return "A fragile charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
            case CharmState.Unbreakable:
                if (RoyalCharmState == 0) return "An unbreakable shell for the royal charm." + Consts.DelicateCharmDesc;
                return "An unbreakable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
        }

        return "";
    }
    
}
