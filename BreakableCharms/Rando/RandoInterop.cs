using ItemChanger.UIDefs;
using Modding.Menu;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace BreakableCharms.Randomizer;

public static class RandoInterop
{
    public static void HookRando()
    {
        BreakableCharms.Instance.Log("Hooking Rando");
        
        RCData.RuntimeLogicOverride.Subscribe(10f, DefineItems);
        RequestBuilder.OnUpdate.Subscribe(20f, AddCharms);
        RandoMenu.AddMenuPage();
    }

    private static void DefineItems(GenerationSettings gs, LogicManagerBuilder lmb)
    {
        if (!BreakableCharms.globalSettings.RandomizeCharmLocations || !gs.PoolSettings.Charms) return;
        
        foreach (var charm in BreakableCharms.RandoCharmList)
        {
            lmb.AddItem(new EmptyItem(charm.name));
        }
    }

    private static void AddCharms(RequestBuilder rb)
    {
        //if charms randoed, rando this too
        if (BreakableCharms.globalSettings.RandomizeCharmLocations && rb.gs.PoolSettings.Charms)
        {
            rb.RemoveItemByName(ItemNames.Unbreakable_Heart);
            rb.RemoveItemByName(ItemNames.Unbreakable_Greed);
            rb.RemoveItemByName(ItemNames.Unbreakable_Strength);
            
            //just add more locations next to original charms to account for the 80ish more items im adding
            foreach (var (_, charmName) in Dictionaries.CharmNameFromID)
            {
                rb.AddLocationByName(CharmLocationLookup(charmName), count: 2);
            }

            //add all my items to the ranomized items
            foreach (var charm in BreakableCharms.RandoCharmList)
            {
                rb.AddItemByName(charm.name);
            }
            
            foreach (var (charmNum, charmName) in Dictionaries.CharmNameFromID)
            {
                rb.EditItemRequest(charmName, info =>
                {
                    info.realItemCreator = (icf, _) =>
                    {
                        var newcharm = icf.MakeItem(charmName);
                        newcharm.name = charmName + Consts.DelicateSuffix;
                        newcharm.UIDef = new MsgUIDef
                        {
                            name = new BoxedString(Language.Language
                                .Get($"{Consts.LangDelicateKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n")),
                            shopDesc = new BoxedString(
                                Language.Language.Get($"{Consts.LangDelicateKey}CHARM_DESC_{charmNum}", "UI")),
                            sprite = new BoxedSprite(Finder.GetItem(charmName).UIDef.GetSprite())
                        };
                            
                        newcharm.tags = new List<Tag>
                        {
                            new ItemChainTag
                            {
                                predecessor = null,
                                successor = charmName + Consts.FragileSuffix
                            }
                        };

                        return newcharm;
                    };
                });
            }
        }
        else
        {
            ItemChangerInterop.AddPlacements();
        }
    }

    public static string CharmLocationLookup(string charmName)
    {
        return charmName switch
        {
            ItemNames.Gathering_Swarm => LocationNames.Sly,
            ItemNames.Wayward_Compass => LocationNames.Iselda,
            ItemNames.Grubsong => LocationNames.Grubfather,
            ItemNames.Stalwart_Shell => LocationNames.Sly,
            ItemNames.Quick_Focus => LocationNames.Salubra,
            ItemNames.Lifeblood_Heart => LocationNames.Salubra,
            ItemNames.Steady_Body => LocationNames.Salubra,
            ItemNames.Heavy_Blow => LocationNames.Sly,
            ItemNames.Longnail => LocationNames.Salubra,
            ItemNames.Shaman_Stone => LocationNames.Salubra,
            ItemNames.Dream_Wielder => LocationNames.Seer,
            ItemNames.Grubberflys_Elegy => LocationNames.Grubfather,
            ItemNames.Sprintmaster => LocationNames.Sly,
            ItemNames.Void_Heart => LocationNames.Arcane_Egg_Birthplace,
            ItemNames.Grimmchild2 => LocationNames.Grimmchild,
            ItemNames.Fragile_Heart => LocationNames.Leg_Eater,
            ItemNames.Fragile_Greed => LocationNames.Leg_Eater,
            ItemNames.Fragile_Strength => LocationNames.Leg_Eater,
            
            _ => charmName,
        };
    }
}