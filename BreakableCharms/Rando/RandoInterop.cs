using ItemChanger.UIDefs;
using Osmi.Game;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.LogicItems.Templates;
using RandomizerMod.Extensions;
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
        
        foreach (var charmName in BreakableCharms.RandoReplacementCharmList.Concat(BreakableCharms.RandoAdditionCharmList).Select(c => c.name))
        {
            lmb.AddTemplateItem(new BranchedItemTemplate(
                Name: charmName,
                Logic: charmName.RemoveSuffix(),
                FalseItem: new MultiItemTemplate(charmName, new[]
                {
                    (charmName.RemoveSuffix(), 1),
                    (Consts.LogicCharms, 1)
                }),
                TrueItem: new SingleItemTemplate(charmName, (charmName.RemoveSuffix(), 1))
            ));
        }

        if (gs.PoolSettings.GrimmkinFlames)
        {
            //grimmchild1

            foreach (var charmName in GrimmChildItems)
            {
                lmb.AddTemplateItem(new CappedItemTemplate(
                    Name: charmName,
                    Effects: new []
                    {
                        ("Grimmchild", 1),
                        (Consts.LogicCharms, 1)
                    },
                    Cap: ("Grimmchild", 1)
                    ));
            }
        }
        else
        {
            //grimmchild2
            
            foreach (var charmName in GrimmChildItems)
            {
                lmb.AddTemplateItem(new CappedItemTemplate(
                    Name: charmName,
                    Effects: new []
                    {
                        ("Grimmchild", 1),
                        (Consts.LogicCharms, 1),
                        ("FLAMES", 6)
                    },
                    Cap: ("Grimmchild", 1)
                ));
            }
        }

        foreach (var charm in RoyalCharmItems)
        {
            lmb.AddItem(new EmptyItem(charm));
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
            foreach (var (_, charmName) in Dictionaries.ICCharmNameFromID)
            {
                rb.AddLocationByName(CharmLocationLookup(charmName), count: 2);
            }

            //add all my items to the ranomized items
            foreach (var charm in BreakableCharms.RandoAdditionCharmList)
            {
                rb.AddItemByName(charm.name);
            }
            
            foreach (var charm in BreakableCharms.RandoReplacementCharmList)
            {
                rb.ReplaceItem(charm.name.RemoveSuffix(), charm.name);
            }

            foreach (var charm in RoyalCharmItems)
            {
                rb.AddItemByName(charm);
            }
            
            ItemChangerInterop.CreateGrimmChildItemsForRando(isGrimmchild1: rb.gs.PoolSettings.GrimmkinFlames);
            
            foreach (var charm in GrimmChildItems)
            {
                if (GrimmChildItems.IndexOf(charm) == 0)
                {
                    rb.ReplaceItem(
                        rb.gs.PoolSettings.GrimmkinFlames ? ItemNames.Grimmchild1 : ItemNames.Grimmchild2,
                        charm);
                }
                else
                {
                    rb.AddItemByName(charm);
                }
            }
            

            foreach (var charmName in BreakableCharms.RandoReplacementCharmList
                         .Concat(BreakableCharms.RandoAdditionCharmList).Select(c => c.name))
            {
                rb.EditItemRequest(charmName, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = charmName,
                        Pool = "Charm",
                        MajorItem = false,
                        PriceCap = 500,
                    };
                });
            }
            
            foreach (var charm in GrimmChildItems)
            {
                rb.EditItemRequest(charm, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = charm,
                        Pool = "Charm",
                        MajorItem = false,
                        PriceCap = 500,
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

    private static List<string> GrimmChildItems = new List<string>()
    {
        "Grimmchild".GetDelicateName(),
        "Grimmchild".GetFragileName(),
        "Grimmchild".GetUnbreakableName()
    };
    private static List<string> RoyalCharmItems = new List<string>()
    {
        Consts.RoyalCharmItemName.GetFragileName(), 
        Consts.RoyalCharmItemName.GetUnbreakableName()
    };
}