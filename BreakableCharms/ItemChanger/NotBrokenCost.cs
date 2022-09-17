namespace BreakableCharms;

public record NotBrokenCost:Cost
{
    public int charmNum;
    public override bool CanPay()
    {
        return !BreakableCharms.localSettings.BrokenCharms[charmNum].isBroken;
    }

    public override void OnPay() { }

    public override string GetCostText()
    {
        return "Charm must not be broken";
    }

    public override bool HasPayEffects() => false;
}