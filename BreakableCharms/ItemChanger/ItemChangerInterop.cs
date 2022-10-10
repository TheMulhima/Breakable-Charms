using ItemChanger.Items;
using ItemChanger.Modules;
using ItemChanger.UIDefs;
using Osmi.Game;

namespace BreakableCharms.ItemChanger;

public static class ItemChangerInterop
{
    public static void AddItems()
    {
        BreakableCharms.ShopCharmList = new List<AbstractItem>();

        foreach (var (charmNum, charmName) in Dictionaries.ICCharmNameFromID)
        {
            BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
            {
                charmNum = charmNum,
                newState = CharmState.Fragile,
                name = charmName.GetFragileName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateShopTagList(charmNum: charmNum, requiredState: CharmState.Delicate, geoCost: 600)
            });
            BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
            {
                charmNum = charmNum,
                newState = CharmState.Unbreakable,
                name = charmName.GetUnbreakableName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateShopTagList(charmNum: charmNum, requiredState: CharmState.Fragile, geoCost: 1500)
            });
            
            var delicateCharm = new BreakableCharmItem
            {
                charmNum = charmNum,
                newState = CharmState.Delicate,
                name = charmName.GetDelicateName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = new List<Tag>
                {
                    new ItemChainTag
                    {
                        predecessor = null,
                        successor = charmName.GetFragileName(),
                    }
                },
            };

            var fragileCharm = new BreakableCharmItem
            {
                charmNum = charmNum,
                newState = CharmState.Fragile,
                name = charmName.GetFragileName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetDelicateName(),
                    successor: charmName.GetUnbreakableName())
            };

            var unbreakableCharm = new BreakableCharmItem
            {
                charmNum = charmNum,
                newState = CharmState.Unbreakable,
                name = charmName.GetUnbreakableName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetFragileName(),
                    successor: null)
            };

            //we only care about this for rando items. for shop items i manually place idrc about defining it
            Finder.DefineCustomItem(delicateCharm);
            Finder.DefineCustomItem(fragileCharm);
            Finder.DefineCustomItem(unbreakableCharm);
            
            AddInteropTag(delicateCharm, charmName.GetDelicateName());
            AddInteropTag(fragileCharm, charmName.GetFragileName());
            AddInteropTag(unbreakableCharm, charmName.GetUnbreakableName());

            BreakableCharms.RandoReplacementCharmList.Add(delicateCharm);
            BreakableCharms.RandoAdditionCharmList.Add(fragileCharm);
            BreakableCharms.RandoAdditionCharmList.Add(unbreakableCharm);
        }

        AddGrimmChildAndRoyalCharmToIC();
        
        //will be called by randointerop
        //CreateGrimmChildItemsForRando();

        CreateRoyalCharmItemsForRando();
    }

    private static void AddInteropTag(AbstractItem item, string message)
    {
        var tag = item.AddTag<InteropTag>();
        tag.Message = message;
        tag.Properties["ModSource"] = BreakableCharms.Instance.Name;
        tag.Properties["PoolGroup"] = "Charm";
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
    
    public static List<Tag> CreateRandoTagList(string predecessor, string successor)
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

    public static void CreateGrimmChildItemsForRando(bool isGrimmchild1)
    {
        string charmName = "Grimmchild";
        int charmNum = (int)Charm.Grimmchild;
        AbstractItem delicateCharm, fragileCharm, unbreakableCharm;

        if (isGrimmchild1)
        {
            delicateCharm = new BreakableGrimmchild1Item
            {
                charmNum = charmNum,
                newState = CharmState.Delicate,
                name = charmName.GetDelicateName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = new List<Tag>
                {
                    new ItemChainTag
                    {
                        predecessor = null,
                        successor = charmName.GetFragileName(),
                    }
                },
            };

            fragileCharm = new BreakableGrimmchild1Item
            {
                charmNum = charmNum,
                newState = CharmState.Fragile,
                name = charmName.GetFragileName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetDelicateName(),
                    successor: charmName.GetUnbreakableName())
            };


            unbreakableCharm = new BreakableGrimmchild1Item
            {
                charmNum = charmNum,
                newState = CharmState.Unbreakable,
                name = charmName.GetUnbreakableName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetFragileName(),
                    successor: null)
            };
        }
        else
        {
            delicateCharm = new BreakableGrimmchild2Item
            {
                charmNum = charmNum,
                newState = CharmState.Delicate,
                name = charmName.GetDelicateName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangDelicateKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = new List<Tag>
                {
                    new ItemChainTag
                    {
                        predecessor = null,
                        successor = charmName.GetFragileName(),
                    }
                },
            };

            fragileCharm = new BreakableGrimmchild2Item
            {
                charmNum = charmNum,
                newState = CharmState.Fragile,
                name = charmName.GetFragileName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetDelicateName(),
                    successor: charmName.GetUnbreakableName())
            };


            unbreakableCharm = new BreakableGrimmchild2Item
            {
                charmNum = charmNum,
                newState = CharmState.Unbreakable,
                name = charmName.GetUnbreakableName(),
                UIDef = new MsgUIDef
                {
                    name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                    shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI")),
                    sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
                },
                tags = CreateRandoTagList(predecessor: charmName.GetFragileName(),
                    successor: null)
            };
        }
        

        Finder.DefineCustomItem(delicateCharm);
        Finder.DefineCustomItem(fragileCharm);
        Finder.DefineCustomItem(unbreakableCharm);
    }
    
    //ignore fragments and only deal with kingsoul
    private static void CreateRoyalCharmItemsForRando()
    {
        string charmName = Consts.RoyalCharmItemName;
        int charmNum = (int)Charm.VoidHeart;
        
        //i dont need a delicate charm. i dont wanna replace any royal charm items

        var fragileCharm = new BreakableCharmItem
        {
            charmNum = charmNum,
            newState = CharmState.Fragile,
            name = charmName.GetFragileName(),
            UIDef = new MsgUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangFragileRoyalCharmName}", "UI").Replace("<br>", "\n")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileRoyalCharmDesc}", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Void_Heart).UIDef.GetSprite())
            },
            tags = CreateRandoTagList(predecessor: null,
                successor: charmName.GetUnbreakableName())
        };


        var unbreakableCharm = new BreakableCharmItem
        {
            charmNum = charmNum,
            newState = CharmState.Unbreakable,
            name = charmName.GetUnbreakableName(),
            UIDef = new MsgUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableRoyalCharmName}", "UI").Replace("<br>", "\n")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableRoyalCharmDesc}", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Void_Heart).UIDef.GetSprite())
            },
            tags = CreateRandoTagList(predecessor: charmName.GetFragileName(),
                successor: null)
        };

        Finder.DefineCustomItem(fragileCharm);
        Finder.DefineCustomItem(unbreakableCharm);
    }

    private static void AddGrimmChildAndRoyalCharmToIC()
    {
        BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
        {
            charmNum = (int)Charm.Grimmchild,
            newState = CharmState.Fragile,
            name = "Fragile Grimmchild",
            UIDef = new MsgUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_40", "UI").Replace("<br>", "\n")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_40", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
            },
            tags = CreateShopTagList(charmNum: (int)Charm.Grimmchild, requiredState: CharmState.Delicate, geoCost: 600)
        });
        BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
        {
            charmNum = (int)Charm.Grimmchild,
            newState = CharmState.Unbreakable,
            name = "Unbreakable Grimmchild",
            UIDef = new MsgUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_40", "UI").Replace("<br>", "\n")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_40", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Grimmchild1).UIDef.GetSprite())
            },
            tags = CreateShopTagList(charmNum: (int)Charm.Grimmchild, requiredState: CharmState.Fragile, geoCost: 1500)
        });
        
        BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
        {
            charmNum = (int)Charm.VoidHeart,
            newState = CharmState.Fragile,
            name = "Fragile Royal Charm",
            UIDef = new SpecialCharmUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_36", "UI").Replace("<br>", "\n")),
                previewName = new BoxedString(Language.Language.Get($"{Consts.LangFragileRoyalCharmName}", "UI")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangFragileRoyalCharmDesc}", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Void_Heart).UIDef.GetSprite())
            },
            tags = CreateShopTagList(charmNum: (int)Charm.VoidHeart, requiredState: CharmState.Delicate, geoCost: 600)
        });
        BreakableCharms.ShopCharmList.Add(new BreakableCharmItem
        {
            charmNum = (int)Charm.VoidHeart,
            newState = CharmState.Unbreakable,
            name = "Unbreakable Royal Charm",
            UIDef = new SpecialCharmUIDef
            {
                name = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_36", "UI").Replace("<br>", "\n")),
                previewName = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableRoyalCharmName}", "UI")),
                shopDesc = new BoxedString(Language.Language.Get($"{Consts.LangUnbreakableRoyalCharmDesc}", "UI")),
                sprite = new BoxedSprite(Finder.GetItem(ItemNames.Void_Heart).UIDef.GetSprite())
            },
            tags = CreateShopTagList(charmNum: (int)Charm.VoidHeart, requiredState: CharmState.Fragile, geoCost: 1500)
        });
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