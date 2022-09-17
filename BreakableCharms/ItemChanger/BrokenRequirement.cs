namespace BreakableCharms;

public class BrokenRequirement : Tag, IShopRequirementTag
{
    public int charmNum;
    
    public bool MeetsRequirement => BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken;
}