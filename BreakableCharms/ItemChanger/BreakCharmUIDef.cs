using ItemChanger.UIDefs;

namespace BreakableCharms;

public class BreakCharmUIDef:MsgUIDef
{
    public override Sprite GetSprite() => Extensions.LoadSpriteFromResources("Images.Misc.BrokenCharm");

    public override string GetPostviewName() => "Charm Broken";
}