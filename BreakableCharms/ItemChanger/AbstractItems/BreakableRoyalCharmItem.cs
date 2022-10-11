namespace BreakableCharms.ItemChanger;

public sealed class BreakableRoyalCharmItem : global::ItemChanger.Items.CharmItem
{
    public CharmState newState;

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = newState;
        
        CharmUtils.SetAllCharmIcons();
    }
    
    public override bool Redundant()
    {
        return (int)BreakableCharms.localSettings.BrokenCharms[charmNum].charmState >= (int)newState;
    }
}
