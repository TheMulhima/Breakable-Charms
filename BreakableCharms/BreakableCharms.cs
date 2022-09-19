using TMPro;

namespace BreakableCharms;

public class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite brokenCharm, geo, charmCostIndicator;
    //some special cases we have
    public static Sprite grimmChild1, grimmChild2, grimmChild3, grimmChild4;
    public static Sprite kingsFragment, queensFragment, kingSoul;

    public static AudioClip charmBuySuccess, charmBuyFail;
    public static AudioSource AudioPlayer;

    public static LocalSettings localSettings { get; private set; } = new LocalSettings();
    public void OnLoadLocal(LocalSettings s) => localSettings = s;
    public LocalSettings OnSaveLocal() => localSettings;
    public static GlobalSettings globalSettings { get; private set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
    public GlobalSettings OnSaveGlobal() => globalSettings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    public override void Initialize()
    {
        Instance ??= this;

        var go = new GameObject();
        UnityEngine.Object.DontDestroyOnLoad(go);
        AudioPlayer = go.AddComponent<AudioSource>();

        brokenCharm = Extensions.LoadSpriteFromResources("Images.Misc.BrokenCharm");
        geo = Extensions.LoadSpriteFromResources("Images.Misc.Geo", 100f);

        On.UIManager.StartNewGame += ICHook;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;
        On.HeroController.Start += UnEquipBrokenCharms;
        On.HeroController.Start += DoFSMEdits;
        On.CharmIconList.Start += FixCharmSprites;
        ModHooks.SceneChanged += FixCharmSpritesOnSceneChange;
        ModHooks.AfterPlayerDeadHook += BreakCharmsOnPlayerDead;
        ModHooks.AfterTakeDamageHook += BreakCharmsOnTakeDamage;
        On.HeroController.HazardRespawn += BreakCharmsOnHazardRespawn;
        On.PlayerData.CountCharms += SetIcons_CountCharms;
        ModHooks.SetPlayerIntHook += SetIcons_SetIntHook;
        ModHooks.GetPlayerIntHook += SetIcons_GetIntHook;
        ModHooks.GetPlayerVariableHook += SetIcons_GetVariableHook;
        ModHooks.SetPlayerBoolHook += DontSetBrokenBools;
        On.CharmDisplay.Start += FixSprites;

        LoadSprites();

        //todo: check if voidheart can be unequipped on death
        //todo: add sound for repairing charms
        //todo: make text grey if people are broke
        //todo: rando integration

        ModHooks.NewGameHook += () =>
        {
            for (int i = 1; i <= 40; i++)
            {
                Ref.PD.SetBoolInternal("gotCharm_" + i, true);
                Ref.PD.hasCharm = true;
                Ref.PD.charmsOwned = 40;
                Ref.PD.royalCharmState = 4;
                Ref.PD.gotShadeCharm = true;
                Ref.PD.charmCost_36 = 0;
                Ref.PD.grimmChildLevel = 5;
                Ref.PD.charmCost_40 = 3;
            }
        };
    }

    private void FixSprites(On.CharmDisplay.orig_Start orig, CharmDisplay self)
    {
        self.brokenGlassHP = localSettings.BrokenCharms[(int)Charm.UnbreakableHeart].GetSprite();
        self.brokenGlassGeo = localSettings.BrokenCharms[(int)Charm.UnbreakableGreed].GetSprite();;
        self.brokenGlassAttack = localSettings.BrokenCharms[(int)Charm.UnbreakableStrength].GetSprite();
        self.whiteCharm = localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        self.blackCharm = localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        orig(self);
        SetAllCharmIcons();
    }

    private bool DontSetBrokenBools(string name, bool orig)
    {
        return !new[]
        {
            nameof(PlayerData.brokenCharm_23),
            nameof(PlayerData.brokenCharm_24),
            nameof(PlayerData.brokenCharm_25)
        }.Contains(name) && orig;
    }

    private void ICHook(On.UIManager.orig_StartNewGame orig, UIManager self, bool permadeath, bool bossrush)
    {
        ItemChangerHook.HookIC();
        orig(self, permadeath, bossrush);
    }

    private void DoFSMEdits(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        FSMEdits.CharmFSMEdits();
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
        SetAllCharmIcons();
    }

    private void FixCharmSprites(On.CharmIconList.orig_Start orig, CharmIconList self)
    {
        orig(self);
        SetAllCharmIcons();
    }

    private void FixCharmSpritesOnSceneChange(string obj)
    {
        SetAllCharmIcons();
    }

    private string ChangeCharmNamesOnBroken(string key, string sheettitle, string orig)
    {
        if (key.Contains("CHARM_NAME"))
        {
            if(localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
            {
                if (key.StartsWith("BreakableCharms"))
                {
                    return charmData.GetShopName(key, sheettitle);
                }
                else
                {
                    return charmData.GetInventoryName(key, sheettitle, orig);
                }

            }
        }
        if (key.Contains("CHARM_DESC"))
        {
            if(localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
            {
                if (key.StartsWith("BreakableCharms"))
                {
                    return charmData.GetShopDesc(key, sheettitle);
                }
                else
                {
                    return charmData.GetInventoryDesc(key, sheettitle, orig);
                }
            }
        }

        return orig;
    }
    
    private void BreakCharmsOnPlayerDead()
    {
        BreakEquippedCharms(s => s is CharmState.Fragile or CharmState.Delicate);
        SetAllCharmIcons();
    }
    
    private int BreakCharmsOnTakeDamage(int hazardtype, int damageamount)
    {
        if (globalSettings.BreakOnAllDamage && damageamount > 0 || globalSettings.BreakOnDoubleDamage && damageamount > 2)
        {
            BreakEquippedCharms(s => s is CharmState.Delicate);
        }
        return damageamount;
    }
    
    private IEnumerator BreakCharmsOnHazardRespawn(On.HeroController.orig_HazardRespawn orig, HeroController self)
    {
        BreakEquippedCharms((s) => s is CharmState.Delicate);
        yield return orig(self);
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
        SetAllCharmIcons();
        
    }
    private void BreakCharm(int charmNum)
    {
        localSettings.BrokenCharms[charmNum].isBroken = true;
        CharmUtil.UnequipCharm(charmNum);
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        
        if(charmNum == (int)Charm.Grimmchild)
        {
            var gc = GameObject.FindWithTag("Grimmchild");
            if (gc != null)
            {
                UnityEngine.Object.Destroy(gc);
            }
            
        }
    }

    public void LoadSprites()
    {
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        AudioClip[] allAudioClips = Resources.FindObjectsOfTypeAll<AudioClip>();
        
        charmCostIndicator = allSprites.First(s => s.name == "charm_UI__0000_charm_cost_02_lit");
        grimmChild1 = allSprites.First(s => s.name == "charm_grimmkin_01");
        grimmChild2 = allSprites.First(s => s.name == "charm_grimmkin_02");
        grimmChild3 = allSprites.First(s => s.name == "charm_grimmkin_03");
        grimmChild4 = allSprites.First(s => s.name == "charm_grimmkin_04");
        kingsFragment = allSprites.First(s => s.name == "charm_white_left");
        queensFragment = allSprites.First(s => s.name == "charm_white_right");
        kingSoul = allSprites.First(s => s.name == "charm_white_full");

        foreach (var (charmNum, spriteName) in Dictionaries.CharmInGameSpriteNameFromID)
        {
            Dictionaries.UnbreakableCharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
        }

        charmBuySuccess = allAudioClips.First(a => a.name == "shiny_item_pickup");
        charmBuyFail = allAudioClips.First(a => a.name == "sword_hit_reject");
    }
    private void SetIcons_CountCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
    {
        orig(self);
        SetAllCharmIcons();
    }
    private int SetIcons_SetIntHook(string name, int orig)
    {
        if (name == nameof(PlayerData.charmsOwned))
        {
            SetAllCharmIcons();
        }
        return orig;
    }
    private int SetIcons_GetIntHook(string name, int orig)
    {
        if (name == nameof(PlayerData.charmsOwned))
        {
            SetAllCharmIcons();
        }
        return orig;
    }
    
    private object SetIcons_GetVariableHook(Type type, string name, object orig)
    {
        if (type == typeof(List<int>) && name == nameof(PlayerData.equippedCharms))
        {
            SetAllCharmIcons();
        }
        return orig;
    }

    public static void SetAllCharmIcons(bool changeDetails = false, int charmNumOfDetails = 0)
    {
        if (CharmIconList.Instance != null)
        {
            CharmIconList.Instance.grimmchildLevel1 = localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel2 = localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel3 = localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.grimmchildLevel4 = localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();
            CharmIconList.Instance.nymmCharm = localSettings.BrokenCharms[(int)Charm.Grimmchild].GetSprite();

            CharmIconList.Instance.unbreakableGreed = localSettings.BrokenCharms[(int)Charm.UnbreakableGreed].GetSprite();
            CharmIconList.Instance.unbreakableHeart = localSettings.BrokenCharms[(int)Charm.UnbreakableHeart].GetSprite();
            CharmIconList.Instance.unbreakableStrength = localSettings.BrokenCharms[(int)Charm.UnbreakableStrength].GetSprite();
        }
        
        foreach (var (charmNum, charmData) in localSettings.BrokenCharms)
        {
            if (CharmIconList.Instance != null)
            {
                CharmIconList.Instance.spriteList[charmNum] = charmData.GetSprite();
            }
            if (FSMEdits.CharmUIGameObject != null) 
            {
                FSMEdits.CharmUIGameObject.transform.Find("Collected Charms").Find(charmNum.ToString()).Find("Sprite").ChangeSpriteRenderer(charmData.GetSprite());
                if (changeDetails && charmNum == charmNumOfDetails)
                {
                    FSMEdits.CharmUIGameObject.transform.Find("Details").Find("Detail Sprite").ChangeSpriteRenderer(charmData.GetSprite());
                    FSMEdits.CharmUIGameObject.transform.Find("Text Desc").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_DESC_{charmNum}", "UI"); 
                    FSMEdits.CharmUIGameObject.transform.Find("Text Name").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_NAME_{charmNum}", "UI");
                }
            }
        }
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;
}