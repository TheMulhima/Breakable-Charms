namespace BreakableCharms.ItemChanger;

public sealed class UnbreakableCharmItem : AbstractItem
{
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = CharmState.Unbreakable;
        CharmUtils.SetAllCharmIcons();
    }

    public override bool Redundant() => false;
}
