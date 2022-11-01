namespace BreakableCharms.Data;

public sealed class HeartData : CharmData
{
    public HeartData():base(23) {}
    public override string GetShopName(string key, string sheettitle)
    {
        if (key.Contains(Consts.LangRepairKey))
        {
            string prefix = charmState switch
            {
                CharmState.UnObtained => "Delicate ",
                CharmState.Delicate => "Delicate ",
                CharmState.Fragile => "Fragile ",
                _ => ""
            };
            return prefix + "Heart" + " (Repair)";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "Delicate Heart";
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "Fragile Heart";
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "Unbreakable Heart";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(Consts.LangRepairKey))
        {
            return "Repair the charm that increases the health of the bearer, allowing them to take more damage.";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "A delicate charm that increases the health of the bearer, allowing them to take more damage." + Consts.DelicateCharmDesc;
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "A fragile charm that increases the health of the bearer, allowing them to take more damage." + Consts.FragileCharmDesc;
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "An unbreakable charm that increases the health of the bearer, allowing them to take more damage." + Consts.UnbreakableCharmDesc;
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Heart";
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                return "Delicate Heart";
            case CharmState.Fragile:
                return "Fragile Heart";
            case CharmState.Unbreakable:
                return "Unbreakable Heart";
        }

        return "";
    }
    
    public override string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                return "A Delicate charm that increases the health of the bearer, allowing them to take more damage." + Consts.DelicateCharmDesc;
            case CharmState.Fragile:
                return "A Fragile charm that increases the health of the bearer, allowing them to take more damage." + Consts.FragileCharmDesc;
            case CharmState.Unbreakable:
                return "An unbreakable charm that increases the health of the bearer, allowing them to take more damage." + Consts.UnbreakableCharmDesc;
        }

        return "";
    }
}