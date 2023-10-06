using ModularSystem.Core;
using ModularSystem.EntityFramework;
using ModularSystem.Mongo;

namespace ModularSystem.Tester;

public class MongoAnt : MongoModel
{
    public CurrencyValue Value { get; set; } = new();
}

public class EFAnt : EFModel
{

}

public class MongoAntEntity : MongoEntity<MongoAnt>
{
    public override IDataAccessObject<MongoAnt> DataAccessObject { get; }

    public MongoAntEntity()
    {
        DataAccessObject = new MongoDataAccessObject<MongoAnt>(DatabaseSource.Ants);
    }
}

public class EFAntEntity : EFEntity<EFAnt>
{
    public override IDataAccessObject<EFAnt> DataAccessObject { get; }

    public EFAntEntity()
    {
        DataAccessObject = new EFCoreDataAccessObject<EFAnt>(EFDatabaseSource.AntsContext());
    }
}