using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents the result of a query, including pagination details and query data.
/// Implements the <see cref="IQueryResult{T}" /> interface.
/// </summary>
/// <typeparam name="T">The type of data contained in the result set.</typeparam>
public class QueryResult<T> : IQueryResult<T>
{
    /// <summary>
    /// Gets or sets the pagination information related to this query result.
    /// </summary>
    public PaginationOut Pagination { get; set; } = new PaginationOut();

    /// <summary>
    /// Gets or sets the data returned by the query.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Checks if the query result contains any data.
    /// </summary>
    /// <returns><c>true</c> if empty; otherwise, <c>false</c>.</returns>
    public bool IsEmpty => Data.IsEmpty();

    /// <summary>
    /// Gets the first item from the data set returned by the query, or null if the set is empty.
    /// </summary>
    [JsonIgnore]
    public T? First => Data.FirstOrDefault();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public QueryResult()
    {
    }

    /// <summary>
    /// Constructor that initializes the query result with data.
    /// </summary>
    /// <param name="data">The data for the query result.</param>
    public QueryResult(List<T> data)
    {
        Data = data;
    }

    /// <summary>
    /// Constructor that initializes the query result with data and pagination details.
    /// </summary>
    /// <param name="data">The data for the query result.</param>
    /// <param name="pagination">The pagination details.</param>
    public QueryResult(IEnumerable<T> data, PaginationOut pagination)
    {
        Data = data.ToList();
        Pagination = pagination;
    }
}
