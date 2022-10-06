namespace BreakableCharms.ItemChanger;

public sealed class FragileCharmItem : AbstractItem
{
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        if (BreakableCharms.localSettings.BrokenCharms[charmNum].charmState < CharmState.Fragile)
        {
            BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = CharmState.Fragile;
        }
        CharmUtils.SetAllCharmIcons();
    }

    public override bool Redundant() => false;
}
