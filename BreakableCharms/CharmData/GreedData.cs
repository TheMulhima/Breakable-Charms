namespace BreakableCharms;

public class GreedData:CharmData
{
    public GreedData():base(24) {}
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
            return prefix + "Greed" + " (Repair)";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "Fragile Greed";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "Unbreakable Greed";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            return "Repair the charm that causes the bearer to find more Geo when defeating enemies.";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            return "A Fragile charm that causes the bearer to find more Geo when defeating enemies.";
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            return "An unbreakable charm that causes the bearer to find more Geo when defeating enemies.";
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Greed";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "Delicate " + "Greed";
            case CharmState.Fragile:
                return "Fragile " + "Greed";
            case CharmState.Unbreakable:
                return "Unbreakable " + "Greed";
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "A Delicate charm that causes the bearer to find more Geo when defeating enemies.";
            case CharmState.Fragile:
                return "A Fragile charm that causes the bearer to find more Geo when defeating enemies.";
            case CharmState.Unbreakable:
                return "An unbreakable charm that causes the bearer to find more Geo when defeating enemies.";
        }

        return "";
    }
}