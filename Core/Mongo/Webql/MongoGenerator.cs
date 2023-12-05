using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Mongo.Webql;

public class MongoGenerator : Generator
{
    public MongoGenerator() : base(GetOptions())
    {

    }

    public MongoGenerator(GeneratorOptions options) : base(options)
    {

    }

    public static GeneratorOptions GetOptions()
    {
        return new()
        {

        };
    }

    public MongoTranslatedQueryable CreateMongoQueryable(Node node, Type type, IQueryable queryable)
    {
        return new MongoTranslatedQueryable(CreateQueryable(node, type, queryable));
    }

    public MongoTranslatedQueryable CreateMongoQueryable<T>(Node node, IQueryable<T> queryable)
    {
        return new MongoTranslatedQueryable(CreateQueryable(node, typeof(T), queryable));
    }
}
