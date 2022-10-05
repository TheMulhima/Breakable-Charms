using ItemChanger.UIDefs;

namespace BreakableCharms;

public class BreakCharmUIDef:MsgUIDef
{
    public override Sprite GetSprite() => SpriteUtils.LoadSpriteFromResources("Misc.BrokenCharm");

    public override string GetPostviewName() => "Charm Broken";
}