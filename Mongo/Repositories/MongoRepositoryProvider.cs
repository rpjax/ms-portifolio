using MongoDB.Driver;
using Aidan.Core.Patterns;

namespace Aidan.Mongo.Repositories;

/// <summary>
/// Provides a MongoDB implementation of the <see cref="IRepositoryProvider"/> interface.
/// </summary>
public class MongoRepositoryProvider : IRepositoryProvider
{
    /// <summary>
    /// Gets the MongoDB database instance used by this provider.
    /// </summary>
    private IMongoDatabase Database { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepositoryProvider"/> class.
    /// </summary>
    /// <param name="database">The MongoDB database instance to use for repository operations.</param>
    public MongoRepositoryProvider(IMongoDatabase database)
    {
        Database = database;
    }

    /// <summary>
    /// Gets a repository for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the repository to get.</typeparam>
    /// <returns>An instance of <see cref="IRepository{T}"/> for the specified type.</returns>
    public IRepository<T> GetRepository<T>()
    {
        var collectionName = typeof(T).Name;
        var collection = Database.GetCollection<T>(collectionName);
        return new MongoRepository<T>(collection);
    }
}
