using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines a data access object for <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object this data access object handles.</typeparam>
public interface IDataAccessObject<T> : IDisposable
{
    /// <summary>
    /// Inserts a record and returns its ID.
    /// </summary>
    /// <param name="data">The record to insert.</param>
    /// <returns>The ID of the inserted record.</returns>
    Task<string> InsertAsync(T data);

    /// <summary>
    /// Inserts multiple records.
    /// </summary>
    /// <param name="entries">The records to insert.</param>
    /// <returns>A task that represents the asynchronous insert operation.</returns>
    Task InsertAsync(IEnumerable<T> entries);

    /// <summary>
    /// Queries the data and returns the results.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>The query results.</returns>
    Task<IQueryResult<T>> QueryAsync(IQuery<T> query);

    /// <summary>
    /// Updates a record.
    /// </summary>
    /// <param name="data">The record to update.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(T data);

    /// <summary>
    /// Updates a specific field in records that match the given selector.
    /// </summary>
    /// <typeparam name="TField">The type of the field to update.</typeparam>
    /// <param name="selector">The selection criteria.</param>
    /// <param name="fieldSelector">The field to update.</param>
    /// <param name="value">The new value.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value);

    /// <summary>
    /// Deletes records that match the given selector.
    /// </summary>
    /// <param name="selector">The selection criteria for deletion.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Expression<Func<T, bool>> selector);

    /// <summary>
    /// Deletes all records.
    /// </summary>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAllAsync();

    /// <summary>
    /// Returns the data as a queryable object.
    /// </summary>
    /// <returns>The queryable object.</returns>
    IQueryable<T> AsQueryable();

    /// <summary>
    /// Counts the number of records that match the given selector.
    /// </summary>
    /// <param name="selector">The selection criteria.</param>
    /// <returns>The number of matching records.</returns>
    Task<long> CountAsync(Expression<Func<T, bool>> selector);

    /// <summary>
    /// Counts all records.
    /// </summary>
    /// <returns>The total number of records.</returns>
    Task<long> CountAllAsync();

    /// <summary>
    /// Validates if the string is in a valid format to the implementation's stringified version of the ID.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns><c>true</c> if the ID format is valid, otherwise <c>false</c>.</returns>
    bool ValidateIdFormat(string id);
}
