using ModularSystem.Core.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines a contract for a repository that manages CRUD operations for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type the repository manages. This type must implement <see cref="IEntity"/>.</typeparam>
public interface IRepository<T> //where T : IEntity
{
    /// <summary>
    /// Provides a way to query entities of type <typeparamref name="T"/> asynchronously, offering the ability to perform LINQ queries over the set of entities.
    /// </summary>
    /// <returns>An asynchronous queryable collection of entities.</returns>
    IAsyncQueryable<T> AsQueryable();

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>A task that represents the asynchronous operation of adding a new entity.</returns>
    Task CreateAsync(T entity);

    /// <summary>
    /// Asynchronously adds multiple new entities to the repository in a single batch operation.
    /// </summary>
    /// <param name="entities">The collection of entities to be added.</param>
    /// <returns>A task that represents the asynchronous operation of adding multiple entities.</returns>
    Task CreateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>A task that represents the asynchronous operation of updating an entity.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Asynchronously updates entities in the repository based on specified update criteria.
    /// </summary>
    /// <param name="update">The criteria used to determine how entities should be updated.</param>
    /// <returns>A task that represents the asynchronous operation of updating entities. The task result is the number of entities updated.</returns>
    Task<long> UpdateAsync(IUpdate<T> update);

    /// <summary>
    /// Asynchronously removes an existing entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    /// <returns>A task that represents the asynchronous operation of removing an entity.</returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Asynchronously removes entities from the repository that match a specified condition.
    /// </summary>
    /// <param name="predicate">An expression that defines the condition entities must meet to be removed.</param>
    /// <returns>A task that represents the asynchronous operation of removing entities. The task result is the number of entities removed.</returns>
    Task<long> DeleteAsync(Expression<Func<T, bool>> predicate);
}
