namespace BreakableCharms;

public static class SpriteUtils
{
    public static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();

    public static Sprite LoadSpriteFromResources(string path, float ppu = 100f)
    {
        if (LoadedSprites.TryGetValue(path, out var loadedSprite)) return loadedSprite;

        var sprite = AssemblyUtils.GetSpriteFromResources("Images." + path + ".png", ppu);
        LoadedSprites[path] = sprite;
        return sprite;
    }
}