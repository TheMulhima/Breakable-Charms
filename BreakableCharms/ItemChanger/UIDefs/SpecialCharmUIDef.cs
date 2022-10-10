using ItemChanger.UIDefs;

namespace BreakableCharms;

public sealed class SpecialCharmUIDef:MsgUIDef
{
    public override string GetPreviewName() => previewName.Value;

    public IString previewName;

    public override UIDef Clone()
    {
        return new SpecialCharmUIDef()
        {
            name = name.Clone(),
            shopDesc = shopDesc.Clone(),
            sprite = sprite.Clone(),
            previewName = previewName.Clone(),
        };
    }
}