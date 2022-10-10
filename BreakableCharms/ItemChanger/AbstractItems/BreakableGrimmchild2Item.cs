using ItemChanger.Util;
using Osmi.Game;
using CharmItem = ItemChanger.Items.CharmItem;

namespace BreakableCharms.ItemChanger;

public sealed class BreakableGrimmchild2Item : global::ItemChanger.Items.Grimmchild2Item
{
    public CharmState newState;

    public override void GiveImmediate(GiveInfo info)
    {
        if (!PlayerData.instance.GetBool(gotBool))
        {
            base.GiveImmediate(info);
        }

        if ((int)BreakableCharms.localSettings.BrokenCharms[charmNum].charmState > (int)newState)
        {
            BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = newState;
        }
        CharmUtils.SetAllCharmIcons();
    }
    
    public override bool Redundant() => BreakableCharms.localSettings.BrokenCharms[charmNum].charmState > newState;
}
