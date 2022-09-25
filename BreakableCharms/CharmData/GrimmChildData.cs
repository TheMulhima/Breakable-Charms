namespace BreakableCharms;

public class GrimmChildData : CharmData
{
    public GrimmChildData():base(40) {}
    public override bool HasSpecialSprite => true;
    public int GrimmChildLevel => PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel));
    public static Dictionary<int, Sprite> BrokenSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_40_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_40_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_40_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_40_4")},
        {5, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_40_5")},
    }; 
    public static Dictionary<int, Sprite> FragileSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_40_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_40_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_40_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_40_4")},
        {5, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_40_5")},
    }; 
    public static Dictionary<int, Sprite> DelicateSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_40_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_40_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_40_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_40_4")},
        {5, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_40_5")},
    }; 
        
    public override Sprite GetSprite()
    {
        if (isBroken)
        {
            return BrokenSprites[GrimmChildLevel];
        }

        if (charmState is CharmState.Delicate)
        {
            return DelicateSprites[GrimmChildLevel];
        }
        if (charmState is CharmState.Fragile)
        {
            return FragileSprites[GrimmChildLevel];
        }
        if (charmState is CharmState.Unbreakable)
        {
            return GrimmChildLevel switch
            {
                1 => BreakableCharms.grimmChild1,
                2 => BreakableCharms.grimmChild2,
                3 => BreakableCharms.grimmChild3,
                4 => BreakableCharms.grimmChild4,
                5 => Dictionaries.UnbreakableCharmSpriteFromID[charmNum]
            };
        }
        return null;
    }
    
}
