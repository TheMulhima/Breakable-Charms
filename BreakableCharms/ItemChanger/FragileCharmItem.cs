using ItemChanger;

namespace BreakableCharms;

public class FragileCharmItem : AbstractItem
{
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        if (BreakableCharms.localSettings.BrokenCharms[charmNum].charmState < CharmState.Fragile)
        {
            BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = CharmState.Fragile;
        }
        BreakableCharms.SetAllCharmIcons();
    }

    public override bool Redundant() => false;
}
