using ItemChanger.Modules;
using ItemChanger.UIDefs;

namespace BreakableCharms.ItemChanger;

public static class ItemChangerInterop
{
    public static void AddItems()
    {
        BreakableCharms.ShopCharmList = new List<AbstractItem>();

        foreach (var (charmNum, charmName) in Dictionaries.CharmNameFromID)
        {
            BreakableCharms.Instance.Log($"{charmName} {charmNum}");
            BreakableCharms.ShopCharmList.Add(new FragileCharmItem
            {
                charmNum = charmNum,
                name = charmName + Consts.FragileSuffix,
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateShopTagList(charmNum: charmNum, requiredState: CharmState.Delicate, geoCost: 600)
            });
            BreakableCharms.ShopCharmList.Add(new UnbreakableCharmItem
            {
                charmNum = charmNum,
                name = charmName + Consts.UnbreakableSuffix,
                UIDef = new MsgUIDef()
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateShopTagList(charmNum: charmNum, requiredState: CharmState.Fragile, geoCost: 1500)
            });

            var fragileCharm = new FragileCharmItem
            {
                charmNum = charmNum,
                name = charmName + Consts.FragileSuffix,
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName + Consts.DelicateSuffix,
                    successor: charmName + Consts.UnbreakableSuffix)
            };

            var unbreakableCharm = new UnbreakableCharmItem
            {
                charmNum = charmNum,
                name = charmName + Consts.UnbreakableSuffix,
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName + Consts.FragileSuffix,
                    successor: null)
            };
            
            //we only care about this for rando items. for shop items i manually place idrc about defining it
            Finder.DefineCustomItem(fragileCharm);
            Finder.DefineCustomItem(unbreakableCharm);
            
            BreakableCharms.RandoCharmList.Add(fragileCharm);
            BreakableCharms.RandoCharmList.Add(unbreakableCharm);
        }
    }

    private static List<Tag> CreateShopTagList(int charmNum, CharmState requiredState, int geoCost)
    {
        List<Tag> list = new ()
        {
            new CostTag
            {
                Cost = new MultiCost(new GeoCost(geoCost),
                    new NotBrokenCost { charmNum = charmNum, })
            },
            new CharmInCorrectStateReq
            {
                charmNum = charmNum,
                requiredState = requiredState
            },
            new SinglePurchaseTag(),
        };
        
        return list;
    }
    
    private static List<Tag> CreateRandoTagList(string predecessor, string successor)
    {
        List<Tag> list = new ()
        {
            new ItemChainTag
            {
                predecessor = predecessor,
                successor = successor
            }
        };
        
        return list;
    }


    public static void AddPlacements()
    {
        BreakableCharms.Instance.Log("Add to leg eater shops");

        ItemChangerMod.AddPlacements(new[]
        {
            new ShopPlacement("Leg_Eater")
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
            }.Add(BreakableCharms.ShopCharmList)
        });

    }
}