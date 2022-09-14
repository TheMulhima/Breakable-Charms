using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Locations;
using ItemChanger.Placements;
using Osmi.Game;
using Satchel.Futils;

namespace BreakableCharms;

public class BreakableCharms : Mod, ILocalSettings<GlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite brokenCharm;

    public static GlobalSettings settings { get; private set; } = new GlobalSettings();
    public void OnLoadLocal(GlobalSettings s) => settings = s;
    public GlobalSettings OnSaveLocal() => settings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    private PlayMakerFSM charmFSM;
    private GameObject CharmUIGameObject => GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;

    public static Dictionary<int, Sprite> CharmSpriteFromID = new();


    public override void Initialize()
    {
        Instance ??= this;

        brokenCharm = AssemblyUtils.GetSpriteFromResources("Images.BrokenCharm.png");

        On.UIManager.StartNewGame += ICHook;

        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;

        //todo: fix sprite on startup
        //todo: fix sprite on fixing (see ck api)
        //todo: message on death

        ModHooks.HeroUpdateHook += () =>
        {
            if (Input.GetKeyDown(KeyCode.Q))
            { 
                for (int i = 1; i <= 30; i++)
                {
                    PlayerData.instance.SetBoolInternal("gotCharm_" + i, true);
                }
            }
        };

        On.HeroController.Start += (orig, self) =>
        {
            orig(self);

            Ref.PD.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
            {
                if (settings.BrokenCharms.ContainsKey(getNameinGS(c)))
                {
                    settings.BrokenCharms[getNameinGS(c)] = true;
                    CharmUtil.UnequipCharm(c);
                    CharmIconList.Instance.spriteList[c] = brokenCharm;
                }
            });
            
            charmFSM = CharmUIGameObject.LocateMyFSM("UI Charms");

            var empty = charmFSM.CopyState("Init", "Empty");
            empty.Actions = Array.Empty<FsmStateAction>();
            empty.Transitions = Array.Empty<FsmTransition>();

            charmFSM.Intercept(new TransitionInterceptor
            {
                fromState = "Deactivate UI",
                eventName = "FINISHED",
                toStateDefault = "Empty", //should intercept is true so it doesnt matter
                toStateCustom = "Empty", //i will handle the cases myself
                shouldIntercept = () => true,
                onIntercept = (_, _) =>
                {
                    var charmNum = charmFSM.GetVariable<FsmInt>("Current Item Number").Value;

                    if (settings.BrokenCharms.TryGetValue(getNameinGS(charmNum), out var broken) && broken)
                    {
                        charmFSM.SetState("Unequippable");
                    }
                    else
                    {
                        charmFSM.SetState("Broken?");
                    }
                    
                }
            });
        };

        On.CharmIconList.Start += (orig,self) =>
        {
            orig(self);
            foreach (var (charm, isBroken) in settings.BrokenCharms.Where(c => c.Value))
            {
                CharmIconList.Instance.spriteList[getNumFromGS(charm)] = brokenCharm;
            }
        };
            

        ModHooks.AfterPlayerDeadHook += () =>
        {
            Ref.PD.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
            {
                if (settings.BrokenCharms.ContainsKey(getNameinGS(c)))
                {
                    settings.BrokenCharms[getNameinGS(c)] = true;
                    CharmUtil.UnequipCharm(c);
                    CharmIconList.Instance.spriteList[c] = brokenCharm;
                }
            });
            
        };
        
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var (charmNum, spriteName) in CharmSpriteNameFromID)
        {
            CharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
        }


    }

    private string ChangeCharmNamesOnBroken(string key, string sheettitle, string orig)
    {
        if (key.Contains("CHARM_NAME"))
        {
            if(settings.BrokenCharms.TryGetValue(getNameinGS(int.Parse(key.Split('_')[2])), out var isBroken) && isBroken)
            {
                return "Broken " + orig;
            }
        }
        if (key.Contains("CHARM_DESC"))
        {
            if(settings.BrokenCharms.TryGetValue(getNameinGS(int.Parse(key.Split('_')[2])), out var isBroken) && isBroken)
            {
                return "A broken charm that " + orig[0].ToString().ToLower() + orig.Substring(1);
            }
        }

        return orig;
    }

    private void ICHook(On.UIManager.orig_StartNewGame orig, UIManager self, bool permadeath, bool bossrush)
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: true, createDefaultModules: false);

        ShopLocation legEaterShopLocation = new()
        {
            dungDiscount = true,
            objectName = "Leg_Eater",
            fsmName = "Conversation Control",
            defaultShopItems = DefaultShopItems.LegEaterCharms | DefaultShopItems.LegEaterRepair,
            name = "Leg_Eater",
            sceneName = "Fungus2_26",
            flingType = FlingType.DirectDeposit,
            tags = null,
            requiredPlayerDataBool = ""
        };

        ShopPlacement LegEaterShopPlacement = new ShopPlacement("Leg_Eater")
        {
            Location = legEaterShopLocation,
            defaultShopItems = DefaultShopItems.LegEaterCharms | DefaultShopItems.LegEaterRepair,
            dungDiscount = true,
            requiredPlayerDataBool = string.Empty
        };
        
        List<AbstractItem> charmList = new List<AbstractItem>();
        
        foreach (var (charm, _) in settings.BrokenCharms)
        {
            int charmNum = getNumFromGS(charm);
            charmList.Add(new BrokenCharmItem(charmNum)
            {
                tags = new List<Tag>
                {
                    new CostTag
                    {
                        Cost = new GeoCost(200)
                    },
                    new ShopReqTag(charmNum)
                }
            });
        }

        LegEaterShopPlacement.Add(charmList);
        
        ItemChangerMod.AddPlacements(new []{LegEaterShopPlacement});


        orig(self, permadeath, bossrush);
    }

    internal static string getNameinGS(int charmNum) => $"charm_{charmNum}_Broken";
    internal static int getNumFromGS(string charmName) => int.Parse(charmName.Split('_')[1]);
    
    public static readonly Dictionary<int, string> CharmNameFromID = new()
    {
        {1, "Gathering_Swarm"},
        {2, "Wayward_Compass"},
        {3, "Grubsong"},
        {4, "Stalwart_Shell"},
        {5, "Baldur_Shell"},
        {6, "Fury_of_the_Fallen"},
        {7, "Quick_Focus"},
        {8, "Lifeblood_Heart"},
        {9, "Lifeblood_Core"},
        {10, "Defender's_Crest"},
        {11, "Flukenest"},
        {12, "Thorns_of_Agony"},
        {13, "Mark_of_Pride"},
        {14, "Steady_Body"},
        {15, "Heavy_Blow"},
        {16, "Sharp_Shadow"},
        {17, "Spore_Shroom"},
        {18, "Longnail"},
        {19, "Shaman_Stone"},
        {20, "Soul_Catcher"},
        {21, "Soul_Eater"},
        {22, "Glowing_Womb"},
        {26, "Nailmaster's_Glory"},
        {27, "Joni's_Blessing"},
        {28, "Shape_of_Unn"},
        {29, "Hiveblood"},
        {30, "Dream_Wielder"},
        {31, "Dashmaster"},
        {32, "Quick_Slash"},
        {33, "Spell_Twister"},
        {34, "Deep_Focus"},
        {35, "Grubberfly's_Elegy"},
        {37, "Sprintmaster"},
        {38, "Dreamshield"},
        {39, "Weaversong"},
        {40, "Grimmchild2"},
    };
    public static readonly Dictionary<int, string> CharmSpriteNameFromID = new()
    {
        { 1, "charm_sprite_02" },
        { 2, "charm_sprite_03" },
        { 3, "charm_grub_mid" },
        { 4, "_0006_charm_stalwart_shell" },
        { 5, "charm_blocker" },
        { 6, "_0005_charm_fury" },
        { 7, "_0005_charm_fast_focus" },
        { 8, "_0010_charm_bluehealth" },
        { 9, "_0007_charm_blue_health_large" },
        { 10, "charm_dung_def" },
        { 11, "charm_fluke" },
        { 12, "_0000_charm_thorn_counter" },
        { 13, "char_mantis" },
        { 14, "_0006_charm_no_recoil" },
        { 15, "_0008_charm_nail_damage_up" },
        { 16, "charm_shade_impact" },
        { 17, "charm_fungus" },
        { 18, "_0007_charm_greed" },
        { 19, "_0002_charm_spell_damage_up" },
        { 20, "_0001_charm_more_soul" },
        { 21, "charm_soul_up_large" },
        { 22, "_0009_charm_Hatchling" },
        { 23, "_0002_charm_glass_heal" },
        { 24, "_0003_charm_glass_geo" },
        { 25, "_0002_charm_glass_attack_up" },
        { 26, "_0004_charm_charge_time_up" },
        { 27, "charm_blue_health_convert" },
        { 28, "charm_slug" },
        { 29, "charm_hive" },
        { 30, "inv_dream_charm" },
        { 31, "_0011_charm_generic_03" },
        { 32, "_0003_charm_nail_slash_speed_up" },
        { 33, "charm_magic_cost_down" },
        { 34, "charm_crystal" },
        { 35, "charm_grub_blade" },
        { 36, "charm_sprite_36" },
        { 37, "charm_grimm_sprint_master" },
        { 38, "charm_grimm_markoth_shield" },
        { 39, "charm_grimm_silkweaver" },
        { 40, "charm_grimmkin_01" },
    };
}