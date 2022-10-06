using ItemChanger.UIDefs;

namespace BreakableCharms;

public sealed class BreakCharmMsgUIDef:MsgUIDef
{
    public override Sprite GetSprite() => SpriteUtils.LoadSpriteFromResources("Misc.BrokenCharm");

    public override string GetPostviewName() => "Charm Broken";
}