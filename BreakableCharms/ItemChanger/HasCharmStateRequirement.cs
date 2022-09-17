namespace BreakableCharms;

public class HasCharmStateRequirement: Tag, IShopRequirementTag
{
    public int charmNum;
    public CharmState requiredState;

    public bool MeetsRequirement =>
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState == requiredState;
}