namespace BreakableCharms;

public static class ItemChangerHook
{
    public static void HookIC()
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: true);

        ShopPlacement LegEaterShopPlacement = new("Leg_Eater")
        {
            Location = new ShopLocation
            {
                dungDiscount = true,
                objectName = "Leg_Eater",
                fsmName = "Conversation Control",
                defaultShopItems = DefaultShopItems.None,
                name = "Leg_Eater",
                sceneName = "Fungus2_26",
                flingType = FlingType.DirectDeposit,
                tags = null,
                requiredPlayerDataBool = ""
            },
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = true,
            requiredPlayerDataBool = string.Empty
        };
        
        List<AbstractItem> charmList = new List<AbstractItem>();

        foreach (var (charmNum, _) in Dictionaries.CharmNameFromID)
        {
            charmList.Add(new FragileCharmItem
            {
                charmNum = charmNum,
                name = Dictionaries.CharmNameFromID[charmNum].Replace("_", " "),
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
                    new HasCharmRequirement
                    {
                        charmNum = charmNum
                    },
                    new HasCharmStateRequirement
                    {
                        charmNum = charmNum,
                        requiredState = CharmState.Delicate,
                    },
                    
                    new ShopPersistentTag
                    {
                        persistence = Persistence.Single
                    },
                    
                }
            });
        }

        foreach (var (charmNum, _) in Dictionaries.CharmNameFromID)
        {
            charmList.Add(new UnbreakableCharmItem
            {
                charmNum = charmNum,
                name = Dictionaries.CharmNameFromID[charmNum].Replace("_", " "),
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
                    new HasCharmRequirement
                    {
                        charmNum = charmNum
                    },
                    new HasCharmStateRequirement
                    {
                        charmNum = charmNum,
                        requiredState = CharmState.Fragile,
                    },
                    new ShopPersistentTag
                    {
                        persistence = Persistence.Single
                    },

                }
            });
        }

        LegEaterShopPlacement.Add(charmList);
        
        ItemChangerMod.AddPlacements(new []{LegEaterShopPlacement});


    }
}