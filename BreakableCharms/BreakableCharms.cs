using TMPro;

namespace BreakableCharms;

public class BreakableCharms : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
{
    internal static BreakableCharms Instance;

    public static Sprite brokenCharm, geo, charmCostIndicator;

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

        brokenCharm = AssemblyUtils.GetSpriteFromResources("Images.BrokenCharm.png");
        geo = AssemblyUtils.GetSpriteFromResources("Images.Geo.png", 100f);

        On.UIManager.StartNewGame += ICHook;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;
        On.HeroController.Start += UnEquipBrokenCharms;
        On.HeroController.Start += DoFSMEdits;
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
            CharmIconList.Instance.spriteList[charm] = brokenCharm;
        }
    }

    private string ChangeCharmNamesOnBroken(string key, string sheettitle, string orig)
    {
        if (key.Contains("CHARM_NAME"))
        {
            if(localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
            {
                if (key.Contains(CharmUIDef.Repair_Key))
                {
                    orig = Extensions.GetOriginalText(key, sheettitle,CharmUIDef.Repair_Key);
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
                    orig = Extensions.GetOriginalText(key, sheettitle,CharmUIDef.Durable_Key);
                    return "Durable " + orig;
                }
                if (key.Contains(CharmUIDef.Unbreakable_Key))
                {
                    orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
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
            if(localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
            {
                if (key.Contains(CharmUIDef.Repair_Key))
                {
                    orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Repair_Key);
                    return "Repair the charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
                }
                if (key.Contains(CharmUIDef.Durable_Key))
                {
                    orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Durable_Key);
                    return "A durable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
                }
                if (key.Contains(CharmUIDef.Unbreakable_Key))
                {
                    orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
                    return "An unbreakable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
                }

                //in charms menu
                if (charmData.isBroken) return "Click enter to repair.\nCost: 200 geo";
                
                switch (charmData.charmState)
                {
                    case CharmState.Fragile:
                        return "A fragile charm that " + orig.MakeFirstCharLower();
                    case CharmState.Durable:
                        return "A durable charm that " + orig.MakeFirstCharLower();
                    case CharmState.Unbreakable:
                        return "An unbreakable charm that " + orig.MakeFirstCharLower();
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
        BreakEquippedCharms((s) => s is CharmState.Fragile);
        yield return orig(self);
    }
    
    //i dont want to import images for no reason
    private void PopulateCharmSpriteFromID()
    {
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        
        charmCostIndicator = allSprites.First(s => s.name == "charm_UI__0000_charm_cost_02_lit");
        
        foreach (var (charmNum, spriteName) in Dictionaries.CharmSpriteNameFromID)
        {
            Dictionaries.CharmSpriteFromID[charmNum] = allSprites.First(s => s.name == spriteName);
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
        
        if(charmNum == (int)Charm.Grimmchild)
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
}