using ModularSystem.Core.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides an abstraction over business operations for entities of type <typeparamref name="T"/>. <br/>
/// Supports CRUD (Create, Read, Update, Delete) actions, data access and querying.
/// </summary>
/// <typeparam name="T">Entity type to be managed by the service.</typeparam>
public interface IEntityService<T> : IDisposable
{
    /// <summary>
    /// Asynchronously creates a single new entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entity">The entity to be persisted.</param>
    /// <returns>The ID of the created entity.</returns>
    Task<string> CreateAsync(T entity);

    /// <summary>
    /// Asynchronously creates multiple entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entities">A collection of entities to be persisted.</param>
    /// <returns>An array of IDs of the created entities.</returns>
    Task<string[]> CreateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Creates an <see cref="IAsyncQueryable{T}"/> instance for querying entities of type <typeparamref name="T"/> asynchronously.
    /// </summary>
    /// <remarks>
    /// This method initializes a queryable interface for the entities, allowing for the construction of asynchronous LINQ queries. <br/>
    /// Use this method to start building queries against the data store without immediately executing them.
    /// </remarks>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> instance that supports building and executing asynchronous queries.</returns>
    IAsyncQueryable<T> CreateQueryable();

    /// <summary>
    /// Queries the data store asynchronously based on the provided query criteria.
    /// </summary>
    /// <param name="query">Criteria for filtering, sorting, and pagination.</param>
    /// <returns>Resulting collection of entities matching the query.</returns>
    Task<IQueryResult<T>> QueryAsync(IQuery<T> query);

    /// <summary>
    /// Updates an existing entity of type <typeparamref name="T"/> asynchronously.
    /// </summary>
    /// <param name="entity">Entity with updated values.</param>
    /// <returns>A task representing the update operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Applies changes to entities that match the criteria defined in <paramref name="update"/>.
    /// </summary>
    /// <param name="update">Contains filter criteria and modifications to be applied.</param>
    /// <returns>A task representing the update operation.</returns>
    Task<long?> UpdateAsync(IUpdate<T> update);

    /// <summary>
    /// Deletes entities that match the provided expression asynchronously.
    /// </summary>
    /// <param name="expression">Criteria to select entities to be deleted.</param>
    /// <returns>A task representing the delete operation.</returns>
    Task<long?> DeleteAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Deletes all entities of type <typeparamref name="T"/> after confirming the operation.
    /// </summary>
    /// <param name="confirmation">A flag to confirm deletion. Set to true to proceed with the delete operation.</param>
    /// <returns>A task representing the delete operation.</returns>
    Task DeleteAllAsync(bool confirmation);
}
