using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines enterprise-level business rules for an entity, including CRUD operations, data access, validation, and querying.
/// </summary>
/// <typeparam name="T">The type of the entity being operated on.</typeparam>
public interface IEntity<T> : IDisposable
{
    /// <summary>
    /// Gets the data access object associated with the entity.
    /// </summary>
    IDataAccessObject<T> DataAccessObject { get; }

    /// <summary>
    /// Gets the validator used for validating the entity before its creation or update.
    /// </summary>
    IValidator<T>? Validator { get; }

    /// <summary>
    /// Gets the validator specifically for update operations on the entity.
    /// </summary>
    IValidator<T>? UpdateValidator { get; }

    /// <summary>
    /// Asynchronously creates a new entity.
    /// </summary>
    /// <param name="entry">The entity to create.</param>
    /// <returns>A task that represents the asynchronous create operation. The task result contains the ID of the created entity.</returns>
    Task<string> CreateAsync(T entry);

    /// <summary>
    /// Asynchronously creates a collection of entities.
    /// </summary>
    /// <param name="entries">The entities to create.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateAsync(IEnumerable<T> entries);

    /// <summary>
    /// Asynchronously retrieves an entity by its ID, if it exists.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A task that returns the retrieved entity, or null if not found.</returns>
    Task<T?> TryGetAsync(string id);

    /// <summary>
    /// Asynchronously retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A task that represents the asynchronous retrieve operation. The task result contains the retrieved entity of type <typeparamref name="T"/>.</returns>
    Task<T> GetAsync(string id);

    /// <summary>
    /// Asynchronously queries entities based on specified criteria.
    /// </summary>
    /// <param name="query">The criteria for querying entities.</param>
    /// <returns>A task that returns a collection of queried entities.</returns>
    Task<IQueryResult<T>> QueryAsync(IQuery<T> query);

    /// <summary>
    /// Asynchronously updates an existing entity.
    /// </summary>
    /// <param name="data">The updated data of the entity.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(T data);

    /// <summary>
    /// Asynchronously deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(string id);

    /// <summary>
    /// Asynchronously deletes entities based on a given predicate.
    /// </summary>
    /// <param name="predicate">The condition to determine which entities to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Asynchronously deletes all entities of the given type.
    /// </summary>
    /// <param name="confirm">A boolean that must be set to true to proceed with the deletion, ensuring safety against accidental deletions.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAllAsync(bool confirm = false);

    /// <summary>
    /// Asynchronously counts entities based on a given predicate.
    /// </summary>
    /// <param name="predicate">The condition to determine which entities to count.</param>
    /// <returns>A task that returns the count of entities that match the predicate.</returns>
    Task<long> CountAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Asynchronously counts all entities of the given type.
    /// </summary>
    /// <returns>A task that returns the total count of entities.</returns>
    Task<long> CountAllAsync();

    /// <summary>
    /// Validates the format of the given entity ID.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>True if the ID format is valid, otherwise false.</returns>
    bool ValidateIdFormat(string id);

    /// <summary>
    /// Asynchronously validates if the given entity ID exists.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>A task that returns true if the ID exists, otherwise false.</returns>
    Task<bool> ValidateIdAsync(string id);
}