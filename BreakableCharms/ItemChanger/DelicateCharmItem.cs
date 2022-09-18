using ItemChanger;

namespace BreakableCharms;

public class DelicateCharmItem : AbstractItem
{
    public int charmNum;
    
    private IEnumerator ResetPaidAfterAFrame()
    {
        yield return null;
        
        ((CostTag)tags[0]).Cost.Paid = false;
        
        ReflectionHelper.SetField<AbstractItem, ObtainState>(this, "obtainState", ObtainState.Unobtained);
    }

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken = false;
        CharmIconList.Instance.spriteList[charmNum] = BreakableCharms.localSettings.BrokenCharms[charmNum].GetSprite();
        GameManager.instance.StartCoroutine(ResetPaidAfterAFrame());
    }

    public override bool Redundant() => false;
}
