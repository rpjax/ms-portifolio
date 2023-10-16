using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Web;
namespace ModularSystem.Mongo;

internal class MongoInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    { 
        JsonSerializerSingleton.TryAddConverter(new ObjectIdJsonConverter());
        ConsoleLogger.Info("Mongo module initialized.");
        return Task.CompletedTask;
    }
}