using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides an abstraction over business operations for entities of type <typeparamref name="T"/>. <br/>
/// Supports CRUD (Create, Read, Update, Delete) actions, data access, validation, and querying.
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
    /// Provides an asynchronous mechanism to generate an initial query for entities of type <typeparamref name="T"/>.
    /// This returned query is flexible and can be further refined or filtered using LINQ before triggering its execution with methods like ToArray() or ToList(), leveraging the deferred execution nature of LINQ.
    /// </summary>
    /// <remarks>
    /// This method offers an entry point to construct dynamic queries for entities. The execution is deferred, meaning that the data store won't be hit until the query is materialized (e.g., by invoking ToList() or ToArray()).
    /// </remarks>
    /// <returns>An IQueryable of type <typeparamref name="T"/> which can be further shaped using LINQ.</returns>
    Task<IQueryable<T>> CreateQueryAsync();

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
    Task UpdateAsync(IUpdate<T> update);

    /// <summary>
    /// Deletes entities that match the provided expression asynchronously.
    /// </summary>
    /// <param name="expression">Criteria to select entities to be deleted.</param>
    /// <returns>A task representing the delete operation.</returns>
    Task DeleteAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Deletes all entities of type <typeparamref name="T"/> after confirming the operation.
    /// </summary>
    /// <param name="confirmation">A flag to confirm deletion. Set to true to proceed with the delete operation.</param>
    /// <returns>A task representing the delete operation.</returns>
    Task DeleteAllAsync(bool confirmation);
}
