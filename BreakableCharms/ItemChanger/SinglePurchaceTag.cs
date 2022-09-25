namespace BreakableCharms;

public class SinglePurchaceTag:Tag, IPersistenceTag
{
    public Persistence Persistence => Persistence.Single;
}