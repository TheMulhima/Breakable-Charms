using TMPro;

namespace BreakableCharms;

public class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite brokenCharm, geo, charmCostIndicator;
    //some special cases we have
    public static Sprite grimmChild1, grimmChild2, grimmChild3, grimmChild4;
    public static Sprite kingsFragment, queensFragment, kingSoul;

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

        brokenCharm = Extensions.LoadSpriteFromResources("Images.BrokenCharm");
        geo = Extensions.LoadSpriteFromResources("Images.Geo", 100f);

        On.UIManager.StartNewGame += ICHook;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;
        On.HeroController.Start += UnEquipBrokenCharms;
        On.HeroController.Start += DoFSMEdits;
        On.CharmIconList.Start += FixCharmSprites;
        ModHooks.AfterPlayerDeadHook += BreakCharmsOnPlayerDead;
        ModHooks.AfterTakeDamageHook += BreakCharmsOnTakeDamage;
        On.HeroController.HazardRespawn += BreakCharmsOnHazardRespawn;

        LoadSprites();

        //todo: make charm fixing UI
        //todo: handle all sprites for gc
        //todo: add special handling for royal charm
        //todo: add special handling for currently fragile charms
        //todo: rename to delicate, fragile, unbreakable
        //todo: rando integration

        ModHooks.NewGameHook += () =>
        {
            for (int i = 1; i <= 40; i++)
            {
                PlayerData.instance.SetBoolInternal("gotCharm_" + i, true);
            }
        };
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
    }

    private void FixCharmSprites(On.CharmIconList.orig_Start orig, CharmIconList self)
    {
        orig(self);
        foreach (var (charm, _) in localSettings.BrokenCharms.Where(c => c.Value.isBroken))
        {
            //todo: handle special cases
            CharmIconList.Instance.spriteList[charm] = localSettings.BrokenCharms[charm].GetSprite();
        }
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
        
    }
    private void BreakCharm(int charmNum)
    {
        localSettings.BrokenCharms[charmNum].isBroken = true;
        CharmUtil.UnequipCharm(charmNum);
        CharmIconList.Instance.spriteList[charmNum] = localSettings.BrokenCharms[charmNum].GetSprite();
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
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) =>
        ModMenu.CreateModMenu(modListMenu);

    public bool ToggleButtonInsideMenu => false;
}