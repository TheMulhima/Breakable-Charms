namespace BreakableCharms;

public static class SpriteUtils
{
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
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}