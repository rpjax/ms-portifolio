using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a contract for managing business operations for entities of type <typeparamref name="T"/>. 
/// This includes CRUD (Create, Read, Update, Delete) operations, data access, validation, and querying.
/// </summary>
/// <typeparam name="T">The type of the entity being operated on.</typeparam>
public interface IEntity<T> : IDisposable
{
    /// <summary>
    /// Asynchronously creates a new entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entry">The entity instance to create.</param>
    /// <returns>A task that represents the asynchronous create operation. The task result contains the ID of the created entity.</returns>
    Task<string> CreateAsync(T entry);

    /// <summary>
    /// Asynchronously retrieves entities based on the specified criteria.
    /// </summary>
    /// <param name="query">An instance of <see cref="IQuery{T}"/> that defines the filtering, sorting, and pagination criteria.</param>
    /// <returns>A task resulting in <see cref="IQueryResult{T}"/> which contains the collection of queried entities.</returns>
    Task<IQueryResult<T>> QueryAsync(IQuery<T> query);

    /// <summary>
    /// Asynchronously updates the details of an existing entity.
    /// </summary>
    /// <param name="data">The updated entity data.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    Task UpdateAsync(T data);

    Task UpdateAsync(Expression expression);

    /// <summary>
    /// Asynchronously removes entities based on a specified condition.
    /// </summary>
    /// <param name="expression">An expression to determine which entities to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync(Expression expression);

    /// <summary>
    /// Asynchronously deletes all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="confirm">A flag to confirm the operation, providing a safeguard against unintended deletions.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAllAsync(bool confirm = false);

    /// <summary>
    /// Asynchronously counts the number of entities that match a given condition.
    /// </summary>
    /// <param name="expression">An expression that determines which entities to include in the count.</param>
    /// <returns>A task that results in the count of entities satisfying the specified condition.</returns>
    Task<long> CountAsync(Expression expression);

    /// <summary>
    /// Asynchronously counts all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A task that, when completed, returns the total count of entities.</returns>
    Task<long> CountAllAsync();

    /// <summary>
    /// Validates the format of the provided entity ID.
    /// </summary>
    /// <param name="id">The identifier to validate.</param>
    /// <returns>True if the ID format is valid; otherwise, false.</returns>
    bool ValidateIdFormat(string id);

    /// <summary>
    /// Asynchronously checks if an entity with the given ID exists.
    /// </summary>
    /// <param name="id">The identifier to validate.</param>
    /// <returns>A task that, when completed, returns true if an entity with the ID exists; otherwise, false.</returns>
    Task<bool> ValidateIdAsync(string id);
}
