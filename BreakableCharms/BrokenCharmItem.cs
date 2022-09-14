using ItemChanger;

namespace BreakableCharms;

public class BrokenCharmItem : AbstractItem
{
    public BrokenCharmItem(int _charmnum)
    {
        charmNum = _charmnum;
        name = BreakableCharms.CharmNameFromID[charmNum].Replace("_", " ");
        UIDef = GetUIDef();
    }
    public int charmNum;

    public override void GiveImmediate(GiveInfo info)
    {
        BreakableCharms.settings.BrokenCharms[BreakableCharms.getNameinGS(charmNum)] = false;
        CharmIconList.Instance.spriteList[charmNum] = BreakableCharms.CharmSpriteFromID[charmNum];
    }

    public override bool Redundant() => false;

    private UIDef GetUIDef()
    {
        var def = Finder.GetItem(BreakableCharms.CharmNameFromID[charmNum]).UIDef;
        //todo: change stuff here
        return def;
    }
}
