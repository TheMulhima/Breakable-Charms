namespace BreakableCharms;

public static class Extensions
{
    #region FSMExtensions
    public static FsmState CreateEmptyState(this PlayMakerFSM fsm, string originalName = "Init", string stateName = "Empty")
    {
        var empty = fsm.CopyState(originalName, stateName);
        empty.Actions = Array.Empty<FsmStateAction>();
        empty.Transitions = Array.Empty<FsmTransition>();
        return empty;
    }
    #endregion

    #region LanguageExtensions
    
    public static string GetOriginalText(string key, string sheettitle, string identifier) => Language.Language.GetInternal(RemoveExcessData(key, identifier), sheettitle);
    public static string RemoveExcessData(this string key, string identifier) => key.Replace("#!#", "").Replace(identifier, "");
    public static int GetCharmNumFromKey(this string key) => int.Parse(key.Split('_')[2]);
    public static string MakeFirstCharLower(this string text) => text[0].ToString().ToLower() + text.Substring(1);
    public static string RemoveSuffix(this string charmName) => charmName.Replace(Consts.DelicateSuffix, "")
        .Replace(Consts.FragileSuffix, "").Replace(Consts.UnbreakableSuffix, "");

    public static string GetDelicateName(this string charmName) => charmName.RemoveSuffix() + Consts.DelicateSuffix;
    public static string GetFragileName(this string charmName) => charmName.RemoveSuffix() + Consts.FragileSuffix;
    public static string GetUnbreakableName(this string charmName) => charmName.RemoveSuffix() + Consts.UnbreakableSuffix;
    
    #endregion

    #region GameObjectExtensions
    
    public static void ChangeSpriteRenderer(this GameObject go, Sprite newSprite) => go.GetComponent<SpriteRenderer>().sprite = newSprite;
    public static void ChangeSpriteRenderer(this Transform transform, Sprite newSprite) => transform.GetComponent<SpriteRenderer>().sprite = newSprite;

    #endregion

    public static int SetPositive(this int value)
    {
        return value >= 0 ? value : 0;
    }
    
}