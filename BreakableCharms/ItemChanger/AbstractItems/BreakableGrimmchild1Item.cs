namespace BreakableCharms.ItemChanger;

public sealed class BreakableGrimmchild1Item : global::ItemChanger.Items.Grimmchild1Item
{
    public CharmState newState;

    public override void GiveImmediate(GiveInfo info)
    {
        if (!PlayerDataAccess.GetBool(gotBool))
        {
            base.GiveImmediate(info);
        }

        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState = newState;
        
        CharmUtils.SetAllCharmIcons();
    }
    
    public override bool Redundant()
    {
        return (int)BreakableCharms.localSettings.BrokenCharms[charmNum].charmState >= (int)newState;
    }
}
