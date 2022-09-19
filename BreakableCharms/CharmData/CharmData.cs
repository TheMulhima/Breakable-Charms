using Logger = Modding.Logger;

namespace BreakableCharms;

public enum CharmState
{
    Delicate = 0,
    Fragile,
    Unbreakable,
}

public class CharmData
{
    public CharmData(int _charmNum)
    {
        charmNum = _charmNum;
        isBroken = false;
        charmState = CharmState.Delicate;
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
        if (isBroken) return Dictionaries.BrokenCharmSpriteFromID[charmNum];
        
        return charmState switch
        {
            CharmState.Delicate => Dictionaries.DelicateCharmSpriteFromID[charmNum],
            CharmState.Fragile => Dictionaries.FragileCharmSpriteFromID[charmNum],
            CharmState.Unbreakable => Dictionaries.UnbreakableCharmSpriteFromID[charmNum]
        };
    }

    public virtual string GetShopName(string key, string sheettitle)
    {
        string orig;
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            orig = Extensions.GetOriginalText(key, sheettitle,CharmUIDef.Repair_Key);
            string prefix = charmState switch
            {
                CharmState.Delicate => "Delicate ",
                CharmState.Fragile => "Fragile ",
                _ => ""
            };
            return prefix + orig + " (Repair)";
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            orig = Extensions.GetOriginalText(key, sheettitle,CharmUIDef.Fragile_Key);
            return "Fragile " + orig;
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
            return "Unbreakable " + orig;
        }

        return "";
    }
    
    public virtual string GetShopDesc(string key, string sheettitle)
    {
        string orig;
        if (key.Contains(CharmUIDef.Repair_Key))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Repair_Key);
            return "Repair the charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
        }
        if (key.Contains(CharmUIDef.Fragile_Key))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Fragile_Key);
            return "A Fragile charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
        }
        if (key.Contains(CharmUIDef.Unbreakable_Key))
        {
            orig = Extensions.GetOriginalText(key,sheettitle,CharmUIDef.Unbreakable_Key);
            return "An unbreakable charm that " + orig.MakeFirstCharLower().Replace("<br>", "\n");
        }

        return "";
    }

    public virtual string GetInventoryName(string key, string sheetitle, string orig)
    {
        if (isBroken) return "Broken " + orig;
                
        switch (charmState)
        {
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
            case CharmState.Delicate:
                return "A Delicate charm that " + orig.MakeFirstCharLower();
            case CharmState.Fragile:
                return "A Fragile charm that " + orig.MakeFirstCharLower();
            case CharmState.Unbreakable:
                return "An unbreakable charm that " + orig.MakeFirstCharLower();
        }

        return "";
    }
}