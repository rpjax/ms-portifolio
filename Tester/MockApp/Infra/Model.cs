using ModularSystem.Core;
using ModularSystem.Core.Security;
using ModularSystem.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace ModularSystem.Tester;

public class Coin : MongoModel
{
    public CurrencyValue CurrencyValue { get; set; } = CurrencyValue.Brl();
}

public class PresentedCoin
{
    public bool IsSoftDeleted { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Owner { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public CurrencyValue CurrencyValue { get; set; } = CurrencyValue.Brl();
}

// Credential Mock
public class Credential : Identity, IMongoModel
{
    public Credential(string uniqueIdentifier) : base(uniqueIdentifier)
    {

    }

    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public ObjectId Id { get; set; }
    public bool IsSoftDeleted { get; set; }
    public string MasterKey { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IQueryableModel? other)
    {
        if (other == null)
        {
            return false;
        }
        return GetId() == other.GetId();
    }

    public string GetId() => Id.ToString();

    public void SetId(string id)
    {
        Id = new ObjectId(id);
    }
}

// paper resource code
[BsonIgnoreExtraElements]
public class Paper : MongoModel, IConversion<PresentedPaper>
{
    public string Text { get; set; } = string.Empty;

    public PresentedPaper Convert()
    {
        return new PresentedPaper();
    }

    public bool SomeFoo() => false;
}

public class PresentedPaper : IConversion<Paper>
{
    public bool IsSoftDeleted { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Paper Convert()
    {
        throw new NotImplementedException();
    }
}

public class UserFile : QueryableModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = "Default Name";

    public override string GetId()
    {
        return Id;
    }

    public override void SetId(string id)
    {
        Id = id;
    }
}

public class Account : MongoModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}