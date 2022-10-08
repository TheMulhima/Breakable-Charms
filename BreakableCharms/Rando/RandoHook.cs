using Modding.Menu;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace BreakableCharms.Randomizer;

public static class RandoHook
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
            //todo: "fix" vanilla charms
            
            //just add more locations next to original charms to account for the 80ish more items im adding
            foreach (var (_, charmName) in Dictionaries.CharmNameFromID)
            {
                rb.AddLocationByName(charmName, count: 2);
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
                        newcharm.UIDef = new CharmUIDef
                        {
                            charmNum = charmNum,
                            StateAfterObtain = CharmState.Delicate,
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
}