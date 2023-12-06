using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Mongo.Webql;

public class MongoGenerator : Translator
{
    public MongoGenerator() : base(GetOptions())
    {

    }

    public MongoGenerator(TranslatorOptions options) : base(options)
    {

    }

    public static TranslatorOptions GetOptions()
    {
        return new()
        {

        };
    }

    public MongoTranslatedQueryable CreateMongoQueryable(Node node, Type type, IQueryable queryable)
    {
        return new MongoTranslatedQueryable(TranslateToQueryable(node, type, queryable));
    }

    public MongoTranslatedQueryable CreateMongoQueryable<T>(Node node, IQueryable<T> queryable)
    {
        return new MongoTranslatedQueryable(TranslateToQueryable(node, typeof(T), queryable));
    }
}
