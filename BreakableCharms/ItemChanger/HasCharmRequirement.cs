namespace BreakableCharms;

public class HasCharmRequirement:Tag, IShopRequirementTag
{
    public int charmNum;

    public bool MeetsRequirement => Ref.PD.GetBool($"gotCharm_{charmNum}");
}