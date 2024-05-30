using ModularSystem.Core.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines a contract for performing data operations on entities of type <typeparamref name="T"/>. <br/>
/// This interface abstracts CRUD operations using LINQ expressions, <br/>
/// offering flexibility to adapt to various data storage mechanisms.
/// </summary>
/// <typeparam name="T">The type of the entity this DAO handles.</typeparam>
public interface IDataAccessObject<T> : IDisposable
{
    /// <summary>
    /// Provides a LINQ-queryable interface to the underlying data source for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A <see cref="IQueryable{T}"/> that can be used to perform LINQ queries on the dataset.</returns>
    IQueryable<T> AsQueryable();

    /// <summary>
    /// Provides an asynchronous, LINQ-queryable interface to the underlying data source for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> that supports constructing and executing asynchronous queries.</returns>
    IAsyncQueryable<T> AsAsyncQueryable();

    /// <summary>
    /// Inserts a single entity into the data storage asynchronously, returning its generated identifier.
    /// </summary>
    /// <param name="data">The entity to insert.</param>
    /// <returns>The identifier of the inserted entity as a string.</returns>
    Task<string> InsertAsync(T data);

    /// <summary>
    /// Inserts a collection of entities into the data storage asynchronously.
    /// </summary>
    /// <param name="entries">Entities to insert.</param>
    /// <returns>A task representing the insertion process.</returns>
    Task InsertAsync(IEnumerable<T> entries);

    /// <summary>
    /// Fetches entities based on the criteria encapsulated in the provided query asynchronously.
    /// </summary>
    /// <param name="query">Criteria to filter, sort, and paginate data.</param>
    /// <returns>A task yielding the results encapsulated in an <see cref="IQueryResult{T}"/>.</returns>
    Task<IQueryResult<T>> QueryAsync(IQuery<T> query);

    /// <summary>
    /// Asynchronously updates the provided entity.
    /// </summary>
    /// <param name="data">Entity with updated values.</param>
    /// <returns>Task representing the update operation.</returns>
    Task UpdateAsync(T data);

    /// <summary>
    /// Updates entities matching the conditions encapsulated in the provided update expression asynchronously.
    /// </summary>
    /// <param name="update">Update criteria and values.</param>
    /// <returns>A task representing the update process.</returns>
    Task<long?> UpdateAsync(IUpdate<T> update);

    /// <summary>
    /// Deletes entities matching the provided criteria asynchronously.
    /// </summary>
    /// <param name="expression">Criteria to identify entities to delete.</param>
    /// <returns>A task representing the deletion process.</returns>
    Task<long?> DeleteAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Purges all entities from the data storage asynchronously.
    /// </summary>
    /// <returns>A task representing the deletion process.</returns>
    Task DeleteAllAsync();

    /// <summary>
    /// Counts the number of entities matching the provided criteria asynchronously.
    /// </summary>
    /// <param name="expression">Criteria to match entities against.</param>
    /// <returns>The count of matching entities.</returns>
    Task<long> CountAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Determines the total number of entities in the data storage asynchronously.
    /// </summary>
    /// <returns>The total entity count.</returns>
    Task<long> CountAllAsync();

    /// <summary>
    /// Validates the format of a given string-based identifier.
    /// </summary>
    /// <param name="id">String representation of the identifier.</param>
    /// <returns>Whether the provided identifier conforms to the expected format.</returns>
    bool ValidateIdFormat(string id);
}