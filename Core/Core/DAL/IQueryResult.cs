namespace ModularSystem.Core;

/// <summary>
/// Defines the structure for the result of a query that is designed to work on data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data the query result will contain.</typeparam>
public interface IQueryResult<T>
{
    /// <summary>
    /// Gets or sets the pagination information for the query result, providing details about the page size and the current page.
    /// </summary>
    PaginationOut Pagination { get; set; }

    /// <summary>
    /// Gets or sets the collection of data elements of type <typeparamref name="T"/> returned by the query.
    /// </summary>
    IEnumerable<T> Data { get; set; }

    /// <summary>
    /// Gets a value indicating whether the query result is empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result is empty; otherwise, <c>false</c>.
    /// </value>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the first element of type <typeparamref name="T"/> in the query result, or null if the result is empty.
    /// </summary>
    /// <value>
    /// The first element in the query result, or null if the result set is empty.
    /// </value>
    T? First { get; }
}
