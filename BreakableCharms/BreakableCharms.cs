using ItemChanger.Modules;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Osmi.Game;
using HKMirror.Hooks.OnHooks;
using HKMirror.Hooks.ILHooks;

namespace BreakableCharms;

public sealed class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<ModGlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite charmCostIndicator;
    //some special cases we have
    public static Sprite grimmChild1, grimmChild2, grimmChild3, grimmChild4;
    public static Sprite kingsFragment, queensFragment, kingSoul;

    public static AudioClip charmBuySuccess, charmBuyFail;
    public static GameObject AudioPlayerPrefab;
    
    //the difference between them is the tags attached
    public static List<AbstractItem> ShopCharmList = new ();
    public static List<AbstractItem> RandoAdditionCharmList = new ();
    public static List<AbstractItem> RandoReplacementCharmList = new ();

    public static LocalSettings localSettings { get; private set; } = new ();
    public void OnLoadLocal(LocalSettings s) => localSettings = s;
    public LocalSettings OnSaveLocal() => localSettings;
    public static ModGlobalSettings globalSettings { get; private set; } = new ();
    public void OnLoadGlobal(ModGlobalSettings s) => globalSettings = s;
    public ModGlobalSettings OnSaveGlobal() => globalSettings;

    public override string GetVersion() => typeof(BreakableCharms).Assembly.GetName().Version.ToString();

    public override void Initialize()
    {
        Instance ??= this;

        OnUIManager.BeforeOrig.StartNewGame += ICHook;
        Osmi.OsmiHooks.AfterEnterSaveHook += FSMEdits.CharmMenuFSMEdits;

        ModHooks.SetPlayerBoolHook += DontSetBrokenCharmBools;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;

        //break charms
        Osmi.OsmiHooks.PlayerDeathHook += _ => CharmUtils.BreakEquippedCharms(s => s is CharmState.Fragile or CharmState.Delicate or CharmState.UnObtained);
        OnHeroController.BeforeOrig.HazardRespawn += _ => CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate or CharmState.UnObtained);
        ILHeroController.TakeDamage += BreakCharmOnDamageTaken;

        //fix sprites
        OnCharmDisplay.WithOrig.Start += FixSprites;
        OnCharmIconList.AfterOrig.Start += _ => CharmUtils.SetAllCharmIcons(); 

        Osmi.OsmiHooks.AfterEnterSaveHook += UnEquipBrokenCharms;

        LoadInGameResources();

        //if they dont have ic ig they'll never have leg eater charms
        if (ModHooks.GetMod(Consts.ICMod) is Mod) ItemChangerInterop.AddItems();
        if (ModHooks.GetMod(Consts.RandoMod) is Mod) RandoInterop.HookRando();
        if (ModHooks.GetMod("RandoSettingsManager") is Mod) RandoSettingsManagerInterop.Hook();
    }

    private void ICHook(OnUIManager.Delegates.Params_StartNewGame args)
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: false);

        //i need legeater to remain where he is
        ItemChangerMod.Modules.GetOrAdd<PreventLegEaterDeath>();
        //if player dies with voidheart, i want it to be unequippable
        ItemChangerMod.Modules.GetOrAdd<RemoveVoidHeartEffects>();
        
        //if it is randomized, RandoHook class will handle it
        if (ModHooks.GetMod(Consts.RandoMod) is not Mod)
        {
            ItemChangerInterop.AddPlacements();
        }
    }

    private void FixSprites(On.CharmDisplay.orig_Start orig, CharmDisplay self)
    {
        self.brokenGlassHP = localSettings.BrokenCharms[(int)Charm.LifebloodHeart].GetSprite();
        self.brokenGlassGeo = localSettings.BrokenCharms[(int)Charm.UnbreakableGreed].GetSprite();
        self.brokenGlassAttack = localSettings.BrokenCharms[(int)Charm.UnbreakableStrength].GetSprite();
        self.whiteCharm = localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        self.blackCharm = localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        orig(self);
        CharmUtils.SetAllCharmIcons();
    }

    private bool DontSetBrokenCharmBools(string name, bool orig)
    {
        return !new[]
        {
            nameof(PlayerDataAccess.brokenCharm_23),
            nameof(PlayerDataAccess.brokenCharm_24),
            nameof(PlayerDataAccess.brokenCharm_25)
        }.Contains(name) && orig;
    }

    private void UnEquipBrokenCharms()
    {
        PlayerDataAccess.equippedCharms.ToList().ForEach(c =>
        {
            if (localSettings.BrokenCharms.ContainsKey(c) && localSettings.BrokenCharms[c].isBroken)
            {
                CharmUtils.BreakCharm(c);
            }
        });
        CharmUtil.UpdateCharm();
        CharmUtils.SetAllCharmIcons();
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
                return charmData.GetInventoryName(key, sheettitle, orig);
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
                return charmData.GetInventoryDesc(key, sheettitle, orig);
            }
        }

        return key switch
        {
            Consts.LangDelicateRoyalCharmName => "Delicate Royal Charm",
            Consts.LangFragileRoyalCharmName => "Fragile Royal Charm",
            Consts.LangUnbreakableRoyalCharmName => "Unbreakable Royal Charm",
            Consts.LangDelicateRoyalCharmDesc => "Obtain the delicate version of royal charm (applies to fragments, kingsoul and voidheart).",
            Consts.LangFragileRoyalCharmDesc => "Obtain the fragile version of royal charm (applies to fragments, kingsoul and voidheart).",
            Consts.LangUnbreakableRoyalCharmDesc => "Obtain the unbreakable version of royal charm (applies to fragments, kingsoul and voidheart).",
            
            _ => orig,
        };
    }
    private void BreakCharmOnDamageTaken(ILContext il)
    {
        ILCursor cursor = new(il);
        //i want this to happen after damage calculated but before applied
        //so go to the line after ModHooks.AfterTakeDamageHook is called
        while (cursor.TryGotoNext(MoveType.After,
                   i =>i.MatchCall<ModHooks>("AfterTakeDamage")))
        {
            cursor.GotoNext(); //go to the line after damage amount is stored in the stack
            cursor.Emit(OpCodes.Ldarg_3); //put the value of damage amount on the stack
            
            cursor.EmitDelegate<Action<int>>((damageamount) =>
            {
                //what game does
                if (PlayerDataAccess.overcharmed)
                {
                    damageamount *= 2;
                }

                if (globalSettings.Break_DelicateCharms_On_AllDamage && damageamount >= 1 ||
                    globalSettings.Break_DelicateCharms_On_DoubleDamage && damageamount >= 2)
                {
                    CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate or CharmState.UnObtained);
                }
            });
        }
    }

    private void LoadInGameResources()
    {
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        AudioClip[] allAudioClips = Resources.FindObjectsOfTypeAll<AudioClip>();
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        charmCostIndicator = allSprites.First(s => s.name == "charm_UI__0000_charm_cost_02_lit");
        grimmChild1 = allSprites.First(s => s.name == "charm_grimmkin_01");
        grimmChild2 = allSprites.First(s => s.name == "charm_grimmkin_02");
        grimmChild3 = allSprites.First(s => s.name == "charm_grimmkin_03");
        grimmChild4 = allSprites.First(s => s.name == "charm_grimmkin_04");
        kingsFragment = allSprites.First(s => s.name == "charm_white_left");
        queensFragment = allSprites.First(s => s.name == "charm_white_right");
        kingSoul = allSprites.First(s => s.name == "charm_white_full");

        foreach (var (charmNum, spriteName) in Dictionaries.InGameSpriteNameFromID)
        {
            Dictionaries.UnbreakableCharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
        }

        charmBuySuccess = allAudioClips.First(a => a.name == "shiny_item_pickup");
        charmBuyFail = allAudioClips.First(a => a.name == "sword_hit_reject");

        AudioPlayerPrefab = allGameObjects.First(a => a.name == "Audio Player Actor");
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;
}