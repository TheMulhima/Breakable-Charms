namespace BreakableCharms;

public class SinglePurchaseTag:Tag, IPersistenceTag
{
    public Persistence Persistence => Persistence.Single;
}