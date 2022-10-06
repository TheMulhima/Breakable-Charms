namespace BreakableCharms;

public sealed class SinglePurchaseTag:Tag, IPersistenceTag
{
    public Persistence Persistence => Persistence.Single;
}