using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Web;
using MongoDB.Bson;

namespace ModularSystem.Mongo;

internal class MongoInitializer : Initializer
{
    public override Task InternalInitAsync(Options options)
    { 
        JsonSerializerSingleton.TryAddConverter(typeof(ObjectId), new ObjectIdJsonConverter());
        ConsoleLogger.Info("Mongo module initialized.");
        return Task.CompletedTask;
    }
}