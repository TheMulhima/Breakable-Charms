namespace BreakableCharms;

public class CharmInCorrectStateReq: Tag, IShopRequirementTag
{
    public int charmNum;
    public CharmState requiredState;

    public bool MeetsRequirement
    {
        get
        {
            //check for if player has charm and it has the required state
            return PlayerData.instance.GetBool($"gotCharm_{charmNum}") &&
                   BreakableCharms.localSettings.BrokenCharms[charmNum].charmState == requiredState;

        }
    }
}