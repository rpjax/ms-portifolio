using MongoDB.Driver;

namespace ModularSystem;

public class MongoModule
{
    public class Database
    {
        public IMongoDatabase Connection { get; set; }
    }

    public static string ConnectionString { get; set; } = "";

    public static IMongoDatabase GetDatabase(string connectionString, string name)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        MongoClient dbClient = new MongoClient(settings);
        return dbClient.GetDatabase(name);
    }

    public static IMongoCollection<T> GetCollection<T>(Database database, string collectionName)
    {
        return database.Connection.GetCollection<T>(collectionName);
    }

    public static async Task Insert<T>(IMongoCollection<T> collection, T data)
    {
        await collection.InsertOneAsync(data);
    }

    public static FilterDefinitionBuilder<T> GetFilterBuilder<T>()
    {
        return Builders<T>.Filter;
    }

    public static UpdateDefinitionBuilder<T> GetUpdateBuilder<T>()
    {
        return Builders<T>.Update;
    }
}