using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Web;
using MongoDB.Bson;

namespace ModularSystem.Mongo;

internal class MongoInitializer : Initializer
{
    public override void OnInit(Options options)
    {
        JsonSerializerSingleton.TryAddConverter(typeof(ObjectId), new ObjectIdConverter());
        SearchEngine.ExpressionSerializer.AddJsonConverter(new NewtonsoftObjectIdConverter());
        ConsoleLogger.Info("Mongo module successfully initialized.");
    }
}