﻿namespace BreakableCharms;

//todo: check if you can requip black charm if unequipped
public class RoyalCharmData : CharmData
{
    public RoyalCharmData():base(36) {}
    public override bool HasSpecialSprite => true;

    public int RoyalCharmState
    {
        get
        {
            //i dont trust people to set royal charm state
            if (Ref.PD.GetBool(nameof(PlayerData.gotShadeCharm))) return 4;
            
            return Ref.PD.GetInt(nameof(PlayerData.royalCharmState));
        }
    }

    public static Dictionary<int, Sprite> BrokenSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_36_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_36_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_36_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Broken.Special.Charm_36_4")},
    }; 
    public static Dictionary<int, Sprite> FragileSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_36_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_36_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_36_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Fragile.Special.Charm_36_4")},
    }; 
    public static Dictionary<int, Sprite> DelicateSprites = new Dictionary<int, Sprite>()
    {
        {1, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_36_1")},
        {2, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_36_2")},
        {3, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_36_3")},
        {4, Extensions.LoadSpriteFromResources("Images.Delicate.Special.Charm_36_4")},
    }; 
        
    public override Sprite GetSprite()
    {
        if (isBroken)
        {
            return BrokenSprites[RoyalCharmState];
        }

        if (charmState is CharmState.Delicate)
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
                1 => BreakableCharms.kingsFragment,
                2 => BreakableCharms.queensFragment,
                3 => BreakableCharms.kingSoul,
                4 => Dictionaries.UnbreakableCharmSpriteFromID[charmNum]
            };
        }
        return null;
    }
    
}
