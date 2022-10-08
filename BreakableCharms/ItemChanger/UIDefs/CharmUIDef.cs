/*using ItemChanger.UIDefs;

namespace BreakableCharms;

public sealed class CharmUIDef:MsgUIDef
{
    public CharmUIDef()
    {
        name = new BoxedString(StateAfterObtain switch
        {
            CharmState.Delicate => Language.Language.Get($"{Consts.LangRepairKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n"),
            CharmState.Fragile => Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n"),
            CharmState.Unbreakable => Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n"),
        });
        shopDesc = new BoxedString(StateAfterObtain switch
        {
            CharmState.Delicate => Language.Language.Get($"{Consts.LangRepairKey}CHARM_DESC_{charmNum}", "UI"),
            CharmState.Fragile => Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI"),
            CharmState.Unbreakable => Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI"),
        });
        sprite = new BoxedSprite(Finder.GetItem(Dictionaries.CharmNameFromID[charmNum]).UIDef.GetSprite());
    }
    public int charmNum;
    public CharmState StateAfterObtain;

    public override UIDef Clone()
    {
        return new CharmUIDef
        {
            charmNum = charmNum,
            StateAfterObtain = StateAfterObtain,
        };
    }
}*/