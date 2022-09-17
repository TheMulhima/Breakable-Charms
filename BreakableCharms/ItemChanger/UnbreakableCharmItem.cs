using ItemChanger;

namespace BreakableCharms;

public class UnbreakableCharmItem : AbstractItem
{
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = CharmState.Unbreakable;
    }

    public override bool Redundant() => false;
}
