namespace BreakableCharms;

public class HasCharmRequirement:Tag, IShopRequirementTag
{
    public int charmNum;

    public bool MeetsRequirement => PlayerData.instance.GetBool($"gotCharm_{charmNum}");
}