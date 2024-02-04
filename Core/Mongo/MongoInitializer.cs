using ModularSystem.Core;
using ModularSystem.Core.Logging;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
namespace ModularSystem.Mongo;

internal class MongoInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        JsonSerializerSingleton.TryAddConverter(new ObjectIdJsonConverter());

        //BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        //BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));

        ConsoleLogger.Info("Mongo module initialized.");
        return Task.CompletedTask;
    }
}