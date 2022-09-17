namespace BreakableCharms;

public class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite brokenCharm;

    public static LocalSettings localSettings { get; private set; } = new LocalSettings();
    public void OnLoadLocal(LocalSettings s) => localSettings = s;
    public LocalSettings OnSaveLocal() => localSettings;
    public static GlobalSettings globalSettings { get; private set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
    public GlobalSettings OnSaveGlobal() => globalSettings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    private PlayMakerFSM charmFSM;
    private static GameObject CharmUIGameObject => GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;

    public static readonly Dictionary<int, Sprite> CharmSpriteFromID = new();


    public override void Initialize()
    {
        Instance ??= this;

        brokenCharm = AssemblyUtils.GetSpriteFromResources("Images.BrokenCharm.png");

        On.UIManager.StartNewGame += ICHook;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;
        On.HeroController.Start += UnEquipBrokenCharms;
        On.HeroController.Start += MakeBrokenCharmsUnEquippable;
        On.CharmIconList.Start += FixCharmSprites;
        ModHooks.AfterPlayerDeadHook += BreakCharmsOnPlayerDead;
        ModHooks.AfterTakeDamageHook += BreakCharmsOnTakeDamage;
        On.HeroController.HazardRespawn += BreakCharmsOnHazardRespawn;

        PopulateCharmSpriteFromID();

        //todo: make charm fixing UI
        //todo: handle all sprites for gc
        //todo: add special handling for royal charm
        //todo: add special handling for currently fragile charms
        //todo: rename to delicate, fragile, unbreakable
        //todo: rando integration
        
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
    }

    private void ICHook(On.UIManager.orig_StartNewGame orig, UIManager self, bool permadeath, bool bossrush)
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: true);

        ShopPlacement LegEaterShopPlacement = new("Leg_Eater")
        {
            Location = new ShopLocation
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
            },
            defaultShopItems = DefaultShopItems.LegEaterCharms | DefaultShopItems.LegEaterRepair,
            dungDiscount = true,
            requiredPlayerDataBool = string.Empty
        };
        
        List<AbstractItem> charmList = new List<AbstractItem>();
        
        //repairing charms
        foreach (var (charmNum, _) in CharmNameFromID)
        {
            charmList.Add(new FragileCharmItem
            {
                charmNum = charmNum,
                name = CharmNameFromID[charmNum].Replace("_", " "),
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Fragile
                },
                tags = new List<Tag>
                {
                    new CostTag
                    {
                        Cost = new GeoCost(200)
                    },
                    new BrokenRequirement
                    {
                        charmNum = charmNum,
                    },
                    new ShopPersistentTag
                    {
                        persistence = Persistence.Persistent
                    },
                }
            });
        }

        foreach (var (charmNum, _) in CharmNameFromID)
        {
            charmList.Add(new DurableCharmItem
            {
                charmNum = charmNum,
                name = CharmNameFromID[charmNum].Replace("_", " "),
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Durable
                },
                tags = new List<Tag>
                {
                    new CostTag()
                    {
                        Cost = new MultiCost(new GeoCost(600),
                            new NotBrokenCost { charmNum = charmNum, })
                    },
                    new HasCharmRequirement()
                    {
                        charmNum = charmNum
                    },

                    new ShopPersistentTag
                    {
                        persistence = Persistence.Single
                    },

                }
            });
        }

        foreach (var (charmNum, _) in CharmNameFromID)
        {
            charmList.Add(new DurableCharmItem
            {
                charmNum = charmNum,
                name = CharmNameFromID[charmNum].Replace("_", " "),
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Durable
                },
                tags = new List<Tag>
                {
                    new CostTag()
                    { 
                        Cost = new MultiCost(new GeoCost(600), 
                        new NotBrokenCost { charmNum = charmNum, })
                    },
                    new HasCharmRequirement()
                    {
                        charmNum = charmNum
                    },
                    new HasCharmStateRequirement()
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

        foreach (var (charmNum, _) in CharmNameFromID)
        {
            charmList.Add(new UnbreakableCharmItem
            {
                charmNum = charmNum,
                name = CharmNameFromID[charmNum].Replace("_", " "),
                UIDef = new CharmUIDef
                {
                    charmNum = charmNum,
                    StateAfterPurchase = CharmState.Unbreakable
                },
                tags = new List<Tag>
                {
                    new CostTag()
                    {
                        Cost = new MultiCost(new GeoCost(1500),
                            new NotBrokenCost { charmNum = charmNum, })
                    },
                    new HasCharmRequirement()
                    {
                        charmNum = charmNum
                    },
                    new HasCharmStateRequirement()
                    {
                        charmNum = charmNum,
                        requiredState = CharmState.Durable,
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


        orig(self, permadeath, bossrush);
    }

    private void MakeBrokenCharmsUnEquippable(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
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

                if (localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) && charmData.isBroken)
                {
                    charmFSM.SetState("Unequippable");
                }
                else
                {
                    charmFSM.SetState("Broken?");
                }
                    
            }
        });
    }

    private void UnEquipBrokenCharms(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        Ref.PD.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
        {
            if (localSettings.BrokenCharms.ContainsKey(c) && localSettings.BrokenCharms[c].isBroken)
            {
                BreakCharm(c);
            }
        });
    }

    private void FixCharmSprites(On.CharmIconList.orig_Start orig, CharmIconList self)
    {
        orig(self);
        foreach (var (charm, _) in localSettings.BrokenCharms.Where(c => c.Value.isBroken))
        {
            CharmIconList.Instance.spriteList[charm] = brokenCharm;
        }
    }

    private string ChangeCharmNamesOnBroken(string key, string sheettitle, string orig)
    {
        if (key.Contains("CHARM_NAME"))
        {
            if(localSettings.BrokenCharms.TryGetValue(GetCharmNumFromKey(key), out var charmData))
            {
                if (key.Contains(CharmUIDef.Repair_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Repair_Key);
                    string prefix = charmData.charmState switch
                    {
                        CharmState.Fragile => "Fragile ",
                        CharmState.Durable => "Durable ",
                        _ => ""
                    };
                    return prefix + orig + " (Repair)";
                }
                if (key.Contains(CharmUIDef.Durable_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Durable_Key);
                    return "Durable " + orig;
                }
                if (key.Contains(CharmUIDef.Unbreakable_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
                    return "Unbreakable " + orig;
                }

                //in charms menu
                if (charmData.isBroken) return "Broken " + orig;
                
                switch (charmData.charmState)
                {
                    case CharmState.Fragile:
                        return "Fragile " + orig;
                    case CharmState.Durable:
                        return "Durable " + orig;
                    case CharmState.Unbreakable:
                        return "Unbreakable " + orig;
                }
            }
        }
        if (key.Contains("CHARM_DESC"))
        {
            if(localSettings.BrokenCharms.TryGetValue(GetCharmNumFromKey(key), out var charmData))
            {
                if (key.Contains(CharmUIDef.Repair_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Repair_Key);
                    return "Repair the charm that " + MakeFirstCharLower(orig).Replace("<br>", "\n");
                }
                if (key.Contains(CharmUIDef.Durable_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Durable_Key);
                    return "A durable charm that " + MakeFirstCharLower(orig).Replace("<br>", "\n");
                }
                if (key.Contains(CharmUIDef.Unbreakable_Key))
                {
                    orig = GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
                    return "An unbreakable charm that " + MakeFirstCharLower(orig).Replace("<br>", "\n");
                }

                //in charms menu
                if(charmData.isBroken) return "A broken charm that " + MakeFirstCharLower(orig);
                
                switch (charmData.charmState)
                {
                    case CharmState.Fragile:
                        return "A fragile charm that " + MakeFirstCharLower(orig);
                    case CharmState.Durable:
                        return "A durable charm that " + MakeFirstCharLower(orig);
                    case CharmState.Unbreakable:
                        return "An unbreakable charm that " + MakeFirstCharLower(orig);
                }
            }
        }

        return orig;
    }
    
    private void BreakCharmsOnPlayerDead()
    {
        BreakEquippedCharms(s => s is CharmState.Durable or CharmState.Fragile);
    }
    
    private int BreakCharmsOnTakeDamage(int hazardtype, int damageamount)
    {
        if (globalSettings.BreakOnAllDamage && damageamount > 0 || globalSettings.BreakOnDoubleDamage && damageamount > 2)
        {
            BreakEquippedCharms(s => s is CharmState.Fragile);
        }
        return damageamount;
    }
    
    private IEnumerator BreakCharmsOnHazardRespawn(On.HeroController.orig_HazardRespawn orig, HeroController self)
    {
        if (globalSettings.BreakOnHazardRespawn)
        {
            BreakEquippedCharms((s) => s is CharmState.Fragile);
        }
        yield return orig(self);
    }
    
    private void PopulateCharmSpriteFromID()
    {
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var (charmNum, spriteName) in CharmSpriteNameFromID)
        {
            CharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
        }
    }

    private void BreakEquippedCharms(Func<CharmState, bool> hasCorrectCharmState)
    {
        bool anyBroken = false;
        Ref.PD.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
        {
            if (localSettings.BrokenCharms.ContainsKey(c) && hasCorrectCharmState(localSettings.BrokenCharms[c].charmState))
            {
                BreakCharm(c);
                anyBroken = true;
            }
        });

        if (anyBroken)
        {
            new BreakCharmUIDef().SendMessage(MessageType.Corner, null);
        }
        
    }
    private void BreakCharm(int charmNum)
    {
        localSettings.BrokenCharms[charmNum].isBroken = true;
        CharmUtil.UnequipCharm(charmNum);
        CharmIconList.Instance.spriteList[charmNum] = brokenCharm;
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        
        if (charmNum == (int)Charm.Grimmchild)
        {
            var gc = GameObject.FindWithTag("Grimmchild");
            if (gc != null)
            {
                UnityEngine.Object.Destroy(gc);
            }
            
        }
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;

    private string GetOriginalText(string key, string sheettitle, string identifier)
    {
        return Language.Language.GetInternal(RemoveExcessData(key, identifier), sheettitle);
    }
    private string RemoveExcessData(string key, string identifier)
    {
        return key.Replace("#!#", "").Replace(identifier, "");
    }
    private int GetCharmNumFromKey(string key)
    {
        return int.Parse(key.Split('_')[2]);
    }
    private string MakeFirstCharLower(string text)
    {
        return text[0].ToString().ToLower() + text.Substring(1);
    }
    
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