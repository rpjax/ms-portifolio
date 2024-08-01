using ModularSystem.Core;
using ModularSystem.Core.Patterns;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ModularSystem.Mongo;

/// <summary>
/// Defines a repository interface for managing MongoDB entities,
/// including support for CRUD operations, <br/>
/// transactions, and entity retrieval by ObjectId. 
/// </summary>
/// <typeparam name="T">The type of the entity managed by the repository. Entities must implement <see cref="IMongoModel"/>,
/// which includes a definition for an ObjectId to serve as the entity's unique identifier.</typeparam>
public interface IMongoRepository<T> : IRepository<T> 
{
    /// <summary>
    /// Binds a MongoDB client session to the repository, enabling support for transactions.
    /// </summary>
    /// <param name="session">The client session handle to bind for transaction support.</param>
    /// <remarks>
    /// This method allows the repository to execute multiple operations within the context of a single transaction, <br/>
    /// ensuring atomicity and consistency of data changes.
    /// </remarks>
    void BindSession(IClientSessionHandle session);

    /// <summary>
    /// Unbinds the currently bound MongoDB client session from the repository, effectively ending transaction support.
    /// </summary>
    /// <remarks>
    /// Call this method to conclude a transactional session, either after committing or aborting the transaction.
    /// </remarks>
    void UnbindSession();

}
