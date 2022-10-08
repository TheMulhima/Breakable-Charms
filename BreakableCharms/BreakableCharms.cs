using ItemChanger.Modules;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Osmi.Game;

namespace BreakableCharms;

public sealed class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
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
    public static List<AbstractItem> RandoCharmList = new ();

    public static LocalSettings localSettings { get; private set; } = new ();
    public void OnLoadLocal(LocalSettings s) => localSettings = s;
    public LocalSettings OnSaveLocal() => localSettings;
    public static GlobalSettings globalSettings { get; private set; } = new ();
    public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
    public GlobalSettings OnSaveGlobal() => globalSettings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    public override void Initialize()
    {
        Instance ??= this;

        On.UIManager.StartNewGame += ICHook;
        Osmi.OsmiHooks.AfterEnterSaveHook += FSMEdits.CharmMenuFSMEdits;

        ModHooks.SetPlayerBoolHook += DontSetBrokenCharmBools;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;
        
        //break charms
        Osmi.OsmiHooks.PlayerDeathHook += BreakCharmsOnPlayerDead;
        On.HeroController.HazardRespawn += BreakCharmsOnHazardRespawn;
        IL.HeroController.TakeDamage += BreakCharmOnDamageTaken; 
        
        //fix sprites
        On.CharmDisplay.Start += FixSprites;
        On.CharmIconList.Start += SetIcons_CharmIconListStart;

        Osmi.OsmiHooks.AfterEnterSaveHook += UnEquipBrokenCharms;

        LoadInGameResources();

        //if they dont have ic ig they'll never have leg eater charms
        if (ModHooks.GetMod(Consts.ICMod) is Mod) ItemChangerInterop.AddItems();
        if (ModHooks.GetMod(Consts.RandoMod) is Mod) RandoInterop.HookRando();

        //todo: check jonis working properly
        //todo: check health in bindings
        //todo: rando integration
    }
    
    private void ICHook(On.UIManager.orig_StartNewGame orig, UIManager self, bool permadeath, bool bossrush)
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
        
        
        orig(self, permadeath, bossrush);
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
            nameof(PlayerData.brokenCharm_23),
            nameof(PlayerData.brokenCharm_24),
            nameof(PlayerData.brokenCharm_25)
        }.Contains(name) && orig;
    }

    private void UnEquipBrokenCharms()
    {
        PlayerData.instance.GetVariable<List<int>>("equippedCharms").ToList().ForEach(c =>
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

        return orig;
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
                if (PlayerData.instance.GetBool(nameof(PlayerData.overcharmed)))
                {
                    damageamount *= 2;
                }

                if (globalSettings.BreakOnAllDamage && damageamount >= 1 ||
                    globalSettings.BreakOnDoubleDamage && damageamount >= 2)
                {
                    CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate);
                }
            });
        }
    }
    
    private void BreakCharmsOnPlayerDead(GameObject _)
    {
        CharmUtils.BreakEquippedCharms(s => s is CharmState.Fragile or CharmState.Delicate);
        CharmUtils.SetAllCharmIcons();
    }

    private IEnumerator BreakCharmsOnHazardRespawn(On.HeroController.orig_HazardRespawn orig, HeroController self)
    {
        CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate);
        yield return orig(self);
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

        foreach (var (charmNum, spriteName) in Dictionaries.CharmInGameSpriteNameFromID)
        {
            Dictionaries.UnbreakableCharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
        }

        charmBuySuccess = allAudioClips.First(a => a.name == "shiny_item_pickup");
        charmBuyFail = allAudioClips.First(a => a.name == "sword_hit_reject");

        AudioPlayerPrefab = allGameObjects.First(a => a.name == "Audio Player Actor");
    }
    private void SetIcons_CharmIconListStart(On.CharmIconList.orig_Start orig, CharmIconList self)
    {
        orig(self);
        CharmUtils.SetAllCharmIcons();
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;
}