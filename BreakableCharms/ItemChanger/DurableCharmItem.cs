using ItemChanger;

namespace BreakableCharms;

public class DurableCharmItem : AbstractItem
{
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = CharmState.Durable;
    }

    public override bool Redundant() => false;
}
