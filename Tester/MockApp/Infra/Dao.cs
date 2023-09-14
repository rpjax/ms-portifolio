using ModularSystem.Mongo;

namespace ModularSystem.Tester;

public class CoinDao : MongoDataAccessObject<Coin>
{
    public CoinDao(string owner) : base(DatabaseSource.Coins)
    {

    }
}

public class PaperDao : MongoDataAccessObject<Paper>
{
    public PaperDao() : base(DatabaseSource.Papers)
    {

    }
}

public class CredentialDao : MongoDataAccessObject<Credential>
{
    public CredentialDao() : base(DatabaseSource.Credentials)
    {

    }
}
