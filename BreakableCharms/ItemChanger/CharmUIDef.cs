using ItemChanger.UIDefs;

namespace BreakableCharms;

public class CharmUIDef:MsgUIDef
{
    public int charmNum;
    public CharmState StateAfterPurchase;
    private UIDef originalDef => Finder.GetItem(Dictionaries.CharmNameFromID[charmNum]).UIDef;

    public const string Repair_Key = "BreakableCharms-Repair-Broken-Charm-";
    public const string Durable_Key = "BreakableCharms-Make-Charm-Durable-";
    public const string Unbreakable_Key = "BreakableCharms-Make-Charm-Unbreakable-";

    public override string GetPostviewName()
    {
        switch (StateAfterPurchase)
        {
            case CharmState.Fragile:
                return Language.Language.Get($"{Repair_Key}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
            case CharmState.Durable:
                return Language.Language.Get($"{Durable_Key}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
            case CharmState.Unbreakable:
                return Language.Language.Get($"{Unbreakable_Key}CHARM_NAME_{charmNum}", "UI").Replace("<br>", "\n");
        }

        return "";
    }

    public override string GetPreviewName() => GetPostviewName();

    public override string GetShopDesc()
    {
        switch (StateAfterPurchase)
        {
            case CharmState.Fragile:
                return Language.Language.Get($"{Repair_Key}CHARM_DESC_{charmNum}", "UI");
            case CharmState.Durable:
                return Language.Language.Get($"{Durable_Key}CHARM_DESC_{charmNum}", "UI");
            case CharmState.Unbreakable:
                return Language.Language.Get($"{Unbreakable_Key}CHARM_DESC_{charmNum}", "UI");
        }

        return "";
    }

    public override Sprite GetSprite() => originalDef.GetSprite();
}