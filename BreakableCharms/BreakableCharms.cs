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

    private PlayMakerFSM charmFSM;
    private static GameObject CharmUIGameObject => GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;

    public override void Initialize()
    {
        Instance ??= this;

        brokenCharm = AssemblyUtils.GetSpriteFromResources("Images.BrokenCharm.png");
        geo = AssemblyUtils.GetSpriteFromResources("Images.Geo.png", 100f);

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

    private void MakeBrokenCharmsUnEquippable(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        charmFSM = CharmUIGameObject.LocateMyFSM("UI Charms");
        charmFSM.CreateEmptyState();
        
        var costgo = CharmUIGameObject.transform.Find("Details").Find("Cost");
        var costFSM = costgo.gameObject.LocateMyFSM("Charm Details Cost");
        var empty = costFSM.CreateEmptyState();

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
                    localSettings.BrokenCharms[charmNum].isBroken = false;
                    CharmIconList.Instance.spriteList[charmNum] = Dictionaries.CharmSpriteFromID[charmNum];
                    CharmUIGameObject.transform.Find("Collected Charms").Find(charmNum.ToString()).Find("Sprite").GetComponent<SpriteRenderer>().sprite = Dictionaries.CharmSpriteFromID[charmNum];

                    CharmUIGameObject.transform.Find("Details").Find("Detail Sprite").GetComponent<SpriteRenderer>().sprite = Dictionaries.CharmSpriteFromID[charmNum];
                    CharmUIGameObject.transform.Find("Text Desc").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_DESC_{charmNum}", "UI");
                    CharmUIGameObject.transform.Find("Text Name").GetComponent<TextMeshPro>().text = Language.Language.Get($"CHARM_NAME_{charmNum}", "UI");
                    
                    var notchCost = PlayerData.instance.GetInt($"charmCost_{charmNum}");
                    costgo.localPosition = costgo.localPosition.X(costFSM.GetVariable<FsmFloat>($"{notchCost} X").Value);
                    costgo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
                    costgo.Find($"Cost 1").GetComponent<SpriteRenderer>().sprite = charmCostIndicator;
                    
                    for (int i = 1; i <= 6; i++)
                    {
                        var costx = costgo.Find($"Cost {i}");
                        costx.localPosition = costx.localPosition.Y(costFSM.GetVariable<FsmFloat>(i <= notchCost ? "Present Y" : "Absent Y").Value);
                    }
                    
                    charmFSM.SetState("Unequippable");
                    
                }
                else
                {
                    //next state in chain, skips break //todo: implement fragile charms
                    charmFSM.SetState("Royal?");
                }
                    
            }
        });

        void OnIntercept(string originalEvent)
        {
            var charmNum = charmFSM.GetVariable<FsmInt>("Current Item Number").Value;
            if (localSettings.BrokenCharms.TryGetValue(charmNum, out var charmData) && charmData.isBroken)
            {
                var costText = costgo.Find("Text Cost");
                costgo.localPosition = costgo.localPosition.X(costFSM.GetVariable<FsmFloat>("1 X").Value);
                costText.gameObject.SetActive(true);
                costText.GetComponent<MeshRenderer>().enabled = true;
                foreach (MeshRenderer meshRenderer in costText.GetComponentsInChildren<MeshRenderer>(true))
                {
                    meshRenderer.enabled = true;
                }

                costText.GetComponent<TextMeshPro>().text = "Cost  200";
                
                
                var geoIcon = costgo.Find($"Cost 1");
                geoIcon.localPosition = geoIcon.localPosition.Y(costFSM.GetVariable<FsmFloat>("Present Y").Value + 0.05f);
                geoIcon.GetComponent<SpriteRenderer>().sprite = geo;
                
                for (int i = 1; i <= 6; i++)
                {
                    if (i == 1) continue;
                    var costx = costgo.Find($"Cost {i}");
                    costx.localPosition = costx.localPosition.Y(costFSM.GetVariable<FsmFloat>("Absent Y").Value);
                }
            }

            else
            {
                costgo.Find($"Cost 1").GetComponent<SpriteRenderer>().sprite = charmCostIndicator;
                costgo.Find("Text Cost").GetComponent<TextMeshPro>().text = "Cost";
                costFSM.SetState("Cost " + originalEvent);
            }
        }
        
        //there is a different event for each charm cost
        for (int i = 0; i <= 6; i++)
        {
            costFSM.Intercept(new TransitionInterceptor
            {
                fromState = "Check",
                eventName = i.ToString(),
                toStateDefault = "Empty", //should intercept is true so it doesnt matter
                toStateCustom = "Empty", //i will handle the cases myself
                shouldIntercept = () => true,
                onIntercept = (_, originalEvent) => OnIntercept(originalEvent)
            });
        }
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
        if (globalSettings.BreakOnHazardRespawn)
        {
            BreakEquippedCharms((s) => s is CharmState.Fragile);
        }
        yield return orig(self);
    }
    
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
}