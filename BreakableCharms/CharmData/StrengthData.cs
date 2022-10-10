namespace BreakableCharms.CharmData;

public sealed class StrengthData : CharmData
{
    public StrengthData():base(25) {}
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
            return prefix + "Strength" + " (Repair)";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "Delicate Strength";
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "Fragile Strength";
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "Unbreakable Strength";
        }

        return "";
    }
    
    public override string GetShopDesc(string key, string sheettitle)
    {
        if (key.Contains(Consts.LangRepairKey))
        {
            return "Repair the charm that strengthens the bearer, increasing the damage they deal to enemies with their nail.";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            return "A delicate charm that strengthens the bearer, increasing the damage they deal to enemies with their nail." + Consts.DelicateCharmDesc;
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            return "A fragile charm that strengthens the bearer, increasing the damage they deal to enemies with their nail." + Consts.FragileCharmDesc;
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            return "An unbreakable charm that strengthens the bearer, increasing the damage they deal to enemies with their nail." + Consts.UnbreakableCharmDesc;
        }

        return "";
    }

    public override string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken Strength";
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                return "Delicate Strength";
            case CharmState.Fragile:
                return "Fragile Strength";
            case CharmState.Unbreakable:
                return "Unbreakable Strength";
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
                return "A delicate charm that strengthens the bearer, increasing the damage they deal to enemies with their nail."+ Consts.DelicateCharmDesc;
            case CharmState.Fragile:
                return "A fragile charm that strengthens the bearer, increasing the damage they deal to enemies with their nail." + Consts.FragileCharmDesc;
            case CharmState.Unbreakable:
                return "An unbreakable charm that strengthens the bearer, increasing the damage they deal to enemies with their nail." + Consts.UnbreakableCharmDesc;
        }

        return "";
    }
}