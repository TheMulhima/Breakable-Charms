namespace BreakableCharms;

public class HeartData:CharmData
{
    public HeartData():base(23) {}
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
            return prefix + "Heart" + " (Repair)";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "Fragile Heart";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "Unbreakable Heart";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            return "Repair the charm that increases the health of the bearer, allowing them to take more damage.";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "A Fragile charm that increases the health of the bearer, allowing them to take more damage.";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "An unbreakable charm that increases the health of the bearer, allowing them to take more damage.";
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Heart";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "Delicate " + "Heart";
            case CharmState.Fragile:
                return "Fragile " + "Heart";
            case CharmState.Unbreakable:
                return "Unbreakable " + "Heart";
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "A Delicate charm that increases the health of the bearer, allowing them to take more damage.";
            case CharmState.Fragile:
                return "A Fragile charm that increases the health of the bearer, allowing them to take more damage.";
            case CharmState.Unbreakable:
                return "An unbreakable charm that increases the health of the bearer, allowing them to take more damage.";
        }

        return "";
    }
}