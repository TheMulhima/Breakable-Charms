using ItemChanger.UIDefs;

namespace BreakableCharms;

public class BreakCharmUIDef:MsgUIDef
{
    public override Sprite GetSprite() => BreakableCharms.brokenCharm;

    public override string GetPostviewName() => "Charm Broken";
}