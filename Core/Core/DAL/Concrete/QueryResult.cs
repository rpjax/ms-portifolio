using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents the result of a query operation, encapsulating the returned data and additional metadata.
/// </summary>
/// <typeparam name="T">The type of data returned by the query.</typeparam>
public class QueryResult<T> : IQueryResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the query result is empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result is empty; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool IsEmpty => Data?.Length == 0;

    /// <summary>
    /// Indicates whether the query result is not empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result contains data; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool IsNotEmpty => !IsEmpty;

    /// <summary>
    /// Gets the total number of data elements of type <typeparamref name="T"/> in the query result.
    /// </summary>
    /// <value>
    /// The total count of data elements in the query result. If the query result is empty, the length is zero.
    /// </value>
    [JsonIgnore]
    public long Length => Data?.LongLength ?? 0;

    /// <summary>
    /// The data elements resulting from the query.
    /// </summary>
    public T[] Data { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Gets the first element of type <typeparamref name="T"/> in the query result, or null if the result is empty.
    /// </summary>
    /// <value>
    /// The first element in the query result, or null if the result set is empty.
    /// </value>
    [JsonIgnore]
    public T? First => Data.FirstOrDefault();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
    /// </summary>
    [JsonConstructor]
    public QueryResult() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with the provided data.
    /// </summary>
    /// <param name="data">The data returned by the query.</param>
    public QueryResult(IEnumerable<T> data)
    {
        Data = data.ToArray();
    }

}
