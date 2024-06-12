using ModularSystem.Core.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a repository contract for data access within the domain.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Gets an asynchronous queryable interface for the repository.
    /// </summary>
    /// <returns>An asynchronous queryable interface.</returns>
    IAsyncQueryable<T> AsQueryable();

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(T entity);

    /// <summary>
    /// Creates multiple entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Updates entities based on the specified update definition asynchronously.
    /// </summary>
    /// <param name="update">The update definition.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<long> UpdateAsync(IUpdate<T> update);

    /// <summary>
    /// Deletes an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Deletes entities based on the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<long> DeleteAsync(Expression<Func<T, bool>> predicate);
}
