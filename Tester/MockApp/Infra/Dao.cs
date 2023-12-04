using ModularSystem.Mongo;

namespace ModularSystem.Tester;

public class CoinDao : MongoDataAccessObject<Coin>
{
    public CoinDao(string owner) : base(MongoDb.Coins)
    {

    }
}

public class PaperDao : MongoDataAccessObject<Paper>
{
    public PaperDao() : base(MongoDb.Papers)
    {

    }
}

public class CredentialDao : MongoDataAccessObject<Credential>
{
    public CredentialDao() : base(MongoDb.Credentials)
    {

    }
}
