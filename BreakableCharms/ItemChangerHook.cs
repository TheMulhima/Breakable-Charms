using ItemChanger.Modules;

namespace BreakableCharms;

public static class ItemChangerHook
{
    public static void HookIC()
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: false);
        
        //i need legeater to remain where he is
        ItemChangerMod.Modules.GetOrAdd<PreventLegEaterDeath>();
        //if player dies with voidheart, i want it to be unequippable
        ItemChangerMod.Modules.GetOrAdd<RemoveVoidHeartEffects>();

        ShopPlacement LegEaterShopPlacement = new("Leg_Eater")
        {
            Location = new ShopLocation
            {
                dungDiscount = true,
                objectName = "Leg_Eater",
                fsmName = "Conversation Control",
                defaultShopItems = DefaultShopItems.LegEaterCharms,
                name = "Leg_Eater",
                sceneName = "Fungus2_26",
                flingType = FlingType.DirectDeposit,
                tags = null,
                requiredPlayerDataBool = ""
            },
            defaultShopItems = DefaultShopItems.LegEaterCharms,
            dungDiscount = true,
            requiredPlayerDataBool = string.Empty
        };
        
        List<AbstractItem> charmList = new List<AbstractItem>();
        
        foreach (var (charmNum, _) in Dictionaries.CharmNameFromID)
        {
            charmList.Add(new FragileCharmItem
            {
                charmNum = charmNum,
                name = Dictionaries.CharmNameFromID[charmNum],
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Fragile
                },
                tags = new List<Tag>
                {
                    new CostTag
                    { 
                        Cost = new MultiCost(new GeoCost(600), 
                        new NotBrokenCost { charmNum = charmNum, })
                    },
                    new CharmInCorrectStateReq
                    {
                        charmNum = charmNum,
                        requiredState = CharmState.Delicate,
                    },
                    
                    new SinglePurchaceTag()
                    
                }
            });
            charmList.Add(new UnbreakableCharmItem
            {
                charmNum = charmNum,
                name = Dictionaries.CharmNameFromID[charmNum],
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Unbreakable
                },
                tags = new List<Tag>
                {
                    new CostTag
                    {
                        Cost = new MultiCost(new GeoCost(1500),
                            new NotBrokenCost { charmNum = charmNum, })
                    },
                    new CharmInCorrectStateReq
                    {
                        charmNum = charmNum,
                        requiredState = CharmState.Fragile,
                    },
                    new SinglePurchaceTag()

                }
            });
        }

        LegEaterShopPlacement.Add(charmList);
        
        ItemChangerMod.AddPlacements(new []{LegEaterShopPlacement});


    }
}