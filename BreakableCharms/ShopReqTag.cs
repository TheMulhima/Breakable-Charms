using ItemChanger;
using ItemChanger.Tags;

namespace BreakableCharms;

public class ShopReqTag : Tag, IShopRequirementTag
{
    public ShopReqTag(int _charmnum)
    {
        charmNum = _charmnum;
    }
    public int charmNum;
    
    public bool MeetsRequirement => BreakableCharms.settings.BrokenCharms[BreakableCharms.getNameinGS(charmNum)];

}