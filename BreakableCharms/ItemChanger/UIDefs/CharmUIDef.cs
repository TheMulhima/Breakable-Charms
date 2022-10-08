using ItemChanger.UIDefs;

namespace BreakableCharms;

public sealed class CharmUIDef:MsgUIDef
{
    public int charmNum;
    public CharmState StateAfterObtain; 

    public override string GetPostviewName()
    {
        switch (StateAfterObtain)
        {
            case CharmState.Delicate:
                return Language.Language.Get($"{Consts.LangRepairKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
            case CharmState.Fragile:
                return Language.Language.Get($"{Consts.LangFragileKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
            case CharmState.Unbreakable:
                return Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
        }

        return "";
    }

    public override string GetPreviewName() => GetPostviewName();

    public override string GetShopDesc()
    {
        switch (StateAfterObtain)
        {
            case CharmState.Delicate:
                return Language.Language.Get($"{Consts.LangRepairKey}CHARM_DESC_{charmNum}", "UI");
            case CharmState.Fragile:
                return Language.Language.Get($"{Consts.LangFragileKey}CHARM_DESC_{charmNum}", "UI");
            case CharmState.Unbreakable:
                return Language.Language.Get($"{Consts.LangUnbreakableKey}CHARM_DESC_{charmNum}", "UI");
        }

        return "";
    }

    public override Sprite GetSprite() => BreakableCharms.localSettings.BrokenCharms[charmNum].GetSprite();
}