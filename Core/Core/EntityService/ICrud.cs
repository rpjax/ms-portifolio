namespace ModularSystem.Core;

/// <summary>
/// Defines a set of basic CRUD (Create, Read, Update, Delete) operations for entities.
/// </summary>
/// <typeparam name="T">The type of the entity being operated on.</typeparam>
public interface ICrud<T>
{
    /// <summary>
    /// Asynchronously creates a new entity.
    /// </summary>
    /// <param name="entry">The entity to create.</param>
    /// <returns>A task that represents the asynchronous create operation. The task result contains the ID of the created entity.</returns>
    Task<string> CreateAsync(T entry);

    /// <summary>
    /// Asynchronously retrieves an entity by its ID, if it exists.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A task that returns the retrieved entity, or null if not found.</returns>
    Task<T?> TryGetAsync(string id);

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
}
