namespace BreakableCharms.CharmData;

public sealed class GreedData : CharmData
{
    public GreedData():base(24) {}
    public override string GetShopName(string key, string sheettitle)
    {
        if (key.Contains(Consts.LangRepairKey))
        {
            string prefix = charmState switch
            {
                CharmState.Delicate => "Delicate ",
                CharmState.Fragile => "Fragile ",
                _ => ""
            };
            return prefix + "Greed" + " (Repair)";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "Delicate Greed";
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "Fragile Greed";
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "Unbreakable Greed";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(Consts.LangRepairKey))
        {
            return "Repair the charm that causes the bearer to find more Geo when defeating enemies.";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "A delicate charm that causes the bearer to find more Geo when defeating enemies." + Consts.DelicateCharmDesc;
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "A fragile charm that causes the bearer to find more Geo when defeating enemies." + Consts.FragileCharmDesc;
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "An unbreakable charm that causes the bearer to find more Geo when defeating enemies." + Consts.UnbreakableCharmDesc;
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Greed";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "Delicate Greed";
            case CharmState.Fragile:
                return "Fragile Greed";
            case CharmState.Unbreakable:
                return "Unbreakable Greed";
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.Delicate:
                return "A Delicate charm that causes the bearer to find more Geo when defeating enemies." + Consts.DelicateCharmDesc;
            case CharmState.Fragile:
                return "A Fragile charm that causes the bearer to find more Geo when defeating enemies."+ Consts.FragileCharmDesc;
            case CharmState.Unbreakable:
                return "An unbreakable charm that causes the bearer to find more Geo when defeating enemies."+ Consts.UnbreakableCharmDesc;
        }

        return "";
    }
}