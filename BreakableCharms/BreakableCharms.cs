using ItemChanger.Modules;
using HKMirror.Hooks.OnHooks;

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

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    public override void Initialize()
    {
        Instance ??= this;

        // the breakable charms module is where most of the edits happen
        OnUIManager.BeforeOrig.StartNewGame += AddModulesAndPlacements;

        LoadInGameResources();

        //if they dont have ic ig they'll never have leg eater charms
        if (ModHooks.GetMod(Consts.ICMod) is Mod) ItemChangerInterop.AddItems();
        if (ModHooks.GetMod(Consts.RandoMod) is Mod) RandoInterop.HookRando();
        if (ModHooks.GetMod("RandoSettingsManager") is Mod) RandoSettingsManagerInterop.Hook();
    }

    private void AddModulesAndPlacements(OnUIManager.Delegates.Params_StartNewGame args)
    {
        ItemChangerMod.CreateSettingsProfile(overwrite: false, createDefaultModules: false);

        //i need legeater to remain where he is
        ItemChangerMod.Modules.GetOrAdd<PreventLegEaterDeath>();
        //if player dies with voidheart, i want it to be unequippable
        ItemChangerMod.Modules.GetOrAdd<RemoveVoidHeartEffects>();
        
        // my module
        ItemChangerMod.Modules.GetOrAdd<BreakableCharmsModule>();
        
        //if it is randomized, RandoHook class will handle it
        if (ModHooks.GetMod(Consts.RandoMod) is not Mod)
        {
            ItemChangerInterop.AddPlacements();
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

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) => ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;
}