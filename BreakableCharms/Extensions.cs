namespace BreakableCharms;

public static class Extensions
{
    public static FsmState CreateEmptyState(this PlayMakerFSM fsm, string originalName = "Init", string stateName = "Empty")
    {
        var empty = fsm.CopyState(originalName, stateName);
        empty.Actions = Array.Empty<FsmStateAction>();
        empty.Transitions = Array.Empty<FsmTransition>();
        return empty;
    }
    
    public static string GetOriginalText(string key, string sheettitle, string identifier) => Language.Language.GetInternal(RemoveExcessData(key, identifier), sheettitle);
    public static string RemoveExcessData(this string key, string identifier) => key.Replace("#!#", "").Replace(identifier, "");
    public static int GetCharmNumFromKey(this string key) => int.Parse(key.Split('_')[2]);
    public static string MakeFirstCharLower(this string text) => text[0].ToString().ToLower() + text.Substring(1);
    
    
    public static Vector3 X (this Vector3 vector3, float x) => new (x, vector3.y, vector3.z);
    public static Vector3 Y (this Vector3 vector3, float y) => new (vector3.x, y, vector3.z);
    public static Vector3 Z (this Vector3 vector3, float z) => new (vector3.x, vector3.y, z);

    public static void ChangeSpriteRenderer(this GameObject go, Sprite newSprite) => go.GetComponent<SpriteRenderer>().sprite = newSprite;
    public static void ChangeSpriteRenderer(this Transform transform, Sprite newSprite) => transform.GetComponent<SpriteRenderer>().sprite = newSprite;

    public static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();

    public static Sprite LoadSpriteFromResources(string path, float ppu = 100f)
    {
        if (LoadedSprites.TryGetValue(path, out var loadedSprite)) return loadedSprite;

        var sprite = GetSpriteFromResources(path + ".png", ppu);
        LoadedSprites[path] = sprite;
        return sprite;
    }
    public static Sprite GetSpriteFromResources(string fileName, float pixelsPerUnit)
    {
        Texture2D texture2D = new Texture2D(2, 2);
        byte[] bytesFromResources = Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
        texture2D.LoadImage(bytesFromResources);
        texture2D.Apply();
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float) texture2D.width, (float) texture2D.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    public static void EditedHCUpdateCharm(int charmNumBroken)
    {
        if (charmNumBroken == (int)Osmi.Game.Charm.NailmastersGlory)
        {
            ReflectionHelper.SetField(HeroController.instance, "nailChargeTime",
            !PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_26))
                ? HeroController.instance.NAIL_CHARGE_TIME_DEFAULT
                : HeroController.instance.NAIL_CHARGE_TIME_CHARM);
        }
        
        if (charmNumBroken == (int) Osmi.Game.Charm.UnbreakableHeart)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.maxHealth), PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)));
            
            if (PlayerData.instance.GetInt(nameof(PlayerData.health)) > PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)))
            {
                PlayerData.instance.SetInt(nameof(PlayerData.health), PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)));    
            }
        }

        if (charmNumBroken == (int)Osmi.Game.Charm.Grimmchild)
        {
            HeroController.instance.carefreeShieldEquipped = PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_40)) && 
                                                             PlayerData.instance.GetInt(nameof(PlayerData.grimmChildLevel)) == 5;
        }

        int oldHealthBlue = PlayerData.instance.GetInt(nameof(PlayerData.healthBlue));
        
        if (charmNumBroken == (int)Osmi.Game.Charm.LifebloodHeart)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.healthBlue), (oldHealthBlue - 2).SetPositive());
        }
        if (charmNumBroken == (int)Osmi.Game.Charm.LifebloodCore)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.healthBlue), (oldHealthBlue - 4).SetPositive());
        }

        if (charmNumBroken == (int)Osmi.Game.Charm.JonisBlessing)
        {
            PlayerData.instance.SetInt(nameof(PlayerData.joniHealthBlue), 0);
            PlayerData.instance.SetInt(nameof(PlayerData.maxHealth), PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)));
            PlayerData.instance.SetInt(nameof(PlayerData.health), PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)));  
            PlayerData.instance.SetBool("joniBeam", false);
        }
    }

    public static int SetPositive(this int value)
    {
        return value >= 0 ? value : 0;
    }
    
}