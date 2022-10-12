namespace BreakableCharms;

public sealed class CharmInCorrectStateReq: Tag, IShopRequirementTag
{
    public int charmNum;
    public CharmState requiredState;

    public bool MeetsRequirement =>
        //check for if player has charm and it has the required state
        PlayerDataAccess.GetBool($"gotCharm_{charmNum}") &&
        BreakableCharms.localSettings.BrokenCharms[charmNum].charmState == requiredState;
}