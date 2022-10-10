using ItemChanger.Util;
using Osmi.Game;
using CharmItem = ItemChanger.Items.CharmItem;

namespace BreakableCharms.ItemChanger;

public sealed class BreakableCharmItem : global::ItemChanger.Items.CharmItem
{
    public CharmState newState;

    public override void GiveImmediate(GiveInfo info)
    {
        if (!PlayerData.instance.GetBool(gotBool))
        {
            base.GiveImmediate(info);
        }
        
        BreakableCharms.Instance.Log($"Giving charm {charmNum} {newState}");

        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = newState;
        
        CharmUtils.SetAllCharmIcons();
    }
    
    public override bool Redundant()
    {
        BreakableCharms.Instance.Log($"Charm {charmNum} {BreakableCharms.localSettings.BrokenCharms[charmNum].charmState} {newState} {(int)BreakableCharms.localSettings.BrokenCharms[charmNum].charmState > (int)newState} ");
        return (int)BreakableCharms.localSettings.BrokenCharms[charmNum].charmState >= (int)newState;
    }
}
