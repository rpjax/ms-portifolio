﻿namespace ModularSystem.Core;

/// <summary>
/// Defines the structure for the result of a query designed to work with data of type <typeparamref name="T"/>.
/// <br/>
/// This interface extends <see cref="IOperationResult{T[]?}"/>, providing additional properties specific to query results.
/// </summary>
/// <typeparam name="T">The type of data contained in the query result.</typeparam>
public interface IQueryResult<T> : IOperationResult<T[]?>
{
    /// <summary>
    /// Gets a value indicating whether the query result is empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result is empty; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// An empty query result can occur either when the query successfully executes but finds no matching records,
    /// <br/>
    /// or if the query fails, in which case <c>Data</c> is null.
    /// </remarks>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the total number of data elements of type <typeparamref name="T"/> in the query result.
    /// </summary>
    /// <value>
    /// The total count of data elements in the query result. If the query result is empty or the query failed, the length is zero.
    /// </value>
    /// <remarks>
    /// This property provides a count of all the items returned by the query, offering a quick way to ascertain the volume of returned data.
    /// </remarks>
    long Length { get; }

    /// <summary>
    /// Gets the first element of type <typeparamref name="T"/> in the query result, or null if the result is empty or the query failed.
    /// </summary>
    /// <value>
    /// The first element in the query result, or null if there are no results or the query failed. This is useful for queries expected to return a single item.
    /// </value>
    /// <remarks>
    /// This property is particularly useful for queries where the focus is on a single result, such as fetching the first or most relevant record.
    /// </remarks>
    T? First { get; }
}
