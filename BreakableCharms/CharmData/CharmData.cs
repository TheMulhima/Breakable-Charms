namespace BreakableCharms.Data;

public enum CharmState
{
    UnObtained = 0,
    Delicate,
    Fragile,
    Unbreakable,
}
/// <summary>
/// The main base class that handles charm naming and sprites. special cases inherit from this class
/// </summary>
public class CharmData
{
    public CharmData(int _charmNum)
    {
        charmNum = _charmNum;
        isBroken = false;
        charmState = CharmState.UnObtained;
    }
    
    public bool isBroken;

    public CharmState charmState;

    public int charmNum;
    
    /// <summary>
    /// for grimm child and void heart
    /// </summary>
    public virtual bool HasSpecialSprite => false;

    public virtual Sprite GetSprite()
    {
        if (Dictionaries.BrokenCharmSpriteFromID.Count == 0) return Dictionaries.UnbreakableCharmSpriteFromID[charmNum];
       
        if (isBroken) return Dictionaries.BrokenCharmSpriteFromID[charmNum];
        
        return charmState switch
        {
            CharmState.Fragile => Dictionaries.FragileCharmSpriteFromID[charmNum],
            CharmState.Unbreakable => Dictionaries.UnbreakableCharmSpriteFromID[charmNum],
            _ => Dictionaries.DelicateCharmSpriteFromID[charmNum],
        };
    }

    public virtual string GetShopName(string key, string sheettitle)
    {
        string orig;
        if (key.Contains(Consts.LangRepairKey))
        {
            orig = Extensions.GetOriginalText(key, sheettitle,Consts.LangRepairKey);
            string prefix = charmState switch
            {
                CharmState.UnObtained => "Delicate ",
                CharmState.Delicate => "Delicate ",
                CharmState.Fragile => "Fragile ",
                _ => ""
            };
            return prefix + orig + " (Repair)";
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            orig = Extensions.GetOriginalText(key, sheettitle,Consts.LangDelicateKey);
            return "Delicate " + orig;
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            orig = Extensions.GetOriginalText(key, sheettitle,Consts.LangFragileKey);
            return "Fragile " + orig;
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,Consts.LangUnbreakableKey);
            return "Unbreakable " + orig;
        }

        return "";
    }
    
    public virtual string GetShopDesc(string key, string sheettitle)
    {
        string orig;
        if (key.Contains(Consts.LangRepairKey))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,Consts.LangRepairKey);
            return "Repair the charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
        }
        if (key.Contains(Consts.LangDelicateKey))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,Consts.LangDelicateKey);
            return "A delicate charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.DelicateCharmDesc;
        }
        if (key.Contains(Consts.LangFragileKey))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,Consts.LangFragileKey);
            return "A fragile charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.FragileCharmDesc;
        }
        if (key.Contains(Consts.LangUnbreakableKey))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,Consts.LangUnbreakableKey);
            return "An unbreakable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.UnbreakableCharmDesc;
        }

        return "";
    }

    public virtual string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken " + orig;
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                return "Delicate " + orig;
            case CharmState.Fragile:
                return "Fragile " + orig;
            case CharmState.Unbreakable:
                return "Unbreakable " + orig;
        }

        return "";
    }
    
    public virtual string GetInventoryDesc(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Click enter to repair.\nCost: 200 geo";
                
        switch (charmState)
        {
            case CharmState.UnObtained:
            case CharmState.Delicate:
                return "A delicate charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.DelicateCharmDesc;
            case CharmState.Fragile:
                return "A fragile charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.FragileCharmDesc;
            case CharmState.Unbreakable:
                return "An unbreakable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n") + Consts.UnbreakableCharmDesc;
        }

        return "";
    }
}