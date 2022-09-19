namespace BreakableCharms;

public class StrengthData:CharmData
{
    public StrengthData():base(25) {}
    public override string GetShopName(string key, string sheettitle)
    {
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            string prefix = charmState switch
            {
                CharmState.Delicate => "Delicate ",
                CharmState.Fragile => "Fragile ",
                _ => ""
            };
            return prefix + "Strength" + " (Repair)";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "Fragile Strength";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "Unbreakable Strength";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            return "Repair the charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "A Fragile charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "An unbreakable charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Strength";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "Delicate " + "Strength";
            case CharmState.Fragile:
                return "Fragile " + "Strength";
            case CharmState.Unbreakable:
                return "Unbreakable " + "Strength";
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "A Delicate charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
            case CharmState.Fragile:
                return "A Fragile charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
            case CharmState.Unbreakable:
                return "An unbreakable charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
        }

        return "";
    }
}