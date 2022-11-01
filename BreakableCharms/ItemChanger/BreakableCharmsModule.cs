using HKMirror.Hooks.ILHooks;
using HKMirror.Hooks.OnHooks;
using ItemChanger.Modules;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Osmi.Game;

namespace BreakableCharms.ItemChanger;

public class BreakableCharmsModule : Module
{
    public override void Initialize()
    {
        Osmi.OsmiHooks.AfterEnterSaveHook += FSMEdits.CharmMenuFSMEdits;

        ModHooks.SetPlayerBoolHook += DontSetBrokenCharmBools;
        ModHooks.LanguageGetHook += ChangeCharmNamesOnBroken;

        //break charms
        Osmi.OsmiHooks.PlayerDeathHook += BreakCharmsOnPlayerDead;
        OnHeroController.BeforeOrig.HazardRespawn += BreakCharmOnHazardRespawn;
        ILHeroController.TakeDamage += BreakCharmOnDamageTaken;

        //fix sprites
        OnCharmDisplay.WithOrig.Start += FixSprites_OnCharmDisplayStart;
        OnCharmIconList.AfterOrig.Start += FixSprites_OnCharmListIconStart;
 
        Osmi.OsmiHooks.AfterEnterSaveHook += UnEquipBrokenCharms;
    }

    public override void Unload()
    {
        Osmi.OsmiHooks.AfterEnterSaveHook -= FSMEdits.CharmMenuFSMEdits;

        ModHooks.SetPlayerBoolHook -= DontSetBrokenCharmBools;
        ModHooks.LanguageGetHook -= ChangeCharmNamesOnBroken;

        //break charms
        Osmi.OsmiHooks.PlayerDeathHook -= BreakCharmsOnPlayerDead;
        OnHeroController.BeforeOrig.HazardRespawn -= BreakCharmOnHazardRespawn;
        ILHeroController.TakeDamage -= BreakCharmOnDamageTaken;

        //fix sprites
        OnCharmDisplay.WithOrig.Start -= FixSprites_OnCharmDisplayStart;
        OnCharmIconList.AfterOrig.Start -= FixSprites_OnCharmListIconStart;

        Osmi.OsmiHooks.AfterEnterSaveHook -= UnEquipBrokenCharms;
    }

    private void FixSprites_OnCharmListIconStart(OnCharmIconList.Delegates.Params_Start args)
    {
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
            if (BreakableCharms.localSettings.BrokenCharms.ContainsKey(c) && BreakableCharms.localSettings.BrokenCharms[c].isBroken)
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
            if(BreakableCharms.localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
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
            if(BreakableCharms.localSettings.BrokenCharms.TryGetValue(key.GetCharmNumFromKey(), out var charmData))
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
    
    private void BreakCharmsOnPlayerDead(GameObject _)
    {
        CharmUtils.BreakEquippedCharms(s => s is CharmState.Fragile or CharmState.Delicate or CharmState.UnObtained);
    }
    
    private void BreakCharmOnHazardRespawn(OnHeroController.Delegates.Params_HazardRespawn _)
    {
        CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate or CharmState.UnObtained);
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

                if (BreakableCharms.globalSettings.Break_DelicateCharms_On_AllDamage && damageamount >= 1 ||
                    BreakableCharms.globalSettings.Break_DelicateCharms_On_DoubleDamage && damageamount >= 2)
                {
                    CharmUtils.BreakEquippedCharms(s => s is CharmState.Delicate or CharmState.UnObtained);
                }
            });
        }
    }
    
    private void FixSprites_OnCharmDisplayStart(On.CharmDisplay.orig_Start orig, CharmDisplay self)
    {
        self.brokenGlassHP = BreakableCharms.localSettings.BrokenCharms[(int)Charm.LifebloodHeart].GetSprite();
        self.brokenGlassGeo = BreakableCharms.localSettings.BrokenCharms[(int)Charm.UnbreakableGreed].GetSprite();
        self.brokenGlassAttack = BreakableCharms.localSettings.BrokenCharms[(int)Charm.UnbreakableStrength].GetSprite();
        self.whiteCharm = BreakableCharms.localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        self.blackCharm = BreakableCharms.localSettings.BrokenCharms[(int)Charm.VoidHeart].GetSprite();
        orig(self);
        CharmUtils.SetAllCharmIcons();
    }
}