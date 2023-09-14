using ModularSystem.Core;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace ModularSystem.Mongo;

public abstract class MongoEntity<T> : Entity<T> where T : class, IMongoModel
{
    public override IValidator<T>? Validator { get; init; }
    public override IValidator<T>? UpdateValidator { get; init; }
    public override IValidator<IQuery<T>>? QueryValidator { get; init; }

    protected MongoEntity()
    {
        Validator = null;
        UpdateValidator = null;
        QueryValidator = null;
    }

    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IMongoModel.Id));
    }

    protected override object? TryParseId(string id)
    {
        if (ObjectId.TryParse(id, out var value))
        {
            return value;
        }

        return null;
    }
}

public class DefaultMongoEntityConfiguration : EntityConfiguration<MongoModel>
{
    public override ISerializer<MongoModel>? GetSerializer()
    {
        return new MongoModelJsonSerializer<MongoModel>();
    }
}

public class MongoEntityConfiguration<T> : DefaultMongoEntityConfiguration where T : class, IMongoModel
{

}
