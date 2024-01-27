using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents the result of a query operation, providing access to the queried data and any associated errors. <br/>
/// Extends <see cref="OperationResult{T[]}"/> to include specific functionality for handling query results.
/// </summary>
/// <typeparam name="T">The type of data returned by the query.</typeparam>
public class QueryResult<T> : OperationResult<T[]>, IQueryResult<T>
{
    /// <summary>
    /// Indicates whether the query result is empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result contains no data; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool IsEmpty => Data?.IsEmpty() == true;

    /// <summary>
    /// Indicates whether the query result is not empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the query result contains data; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool IsNotEmpty => !IsEmpty;

    /// <inheritdoc/>
    public long Length => Data?.LongLength ?? default;

    /// <inheritdoc/>
    [JsonIgnore]
    public T? First => GetFirst();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
    /// </summary>
    [JsonConstructor]
    public QueryResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with data and optional errors.
    /// </summary>
    /// <param name="data">The data returned by the query.</param>
    /// <param name="errors">Optional errors encountered during the query.</param>
    public QueryResult(IEnumerable<T> data, params Error[] errors)
        : base(true, data.ToArray(), errors)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with a set of errors, indicating a failed query.
    /// </summary>
    /// <param name="errors">Errors encountered during the query.</param>
    public QueryResult(params Error[] errors) : base(errors)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with a collection of errors, indicating a failed query.
    /// </summary>
    /// <param name="errors">A collection of errors encountered during the query.</param>
    public QueryResult(IEnumerable<Error> errors) : base(errors)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class based on an existing <see cref="OperationResult{T[]}"/>. 
    /// </summary>
    /// <param name="operationResult">The <see cref="OperationResult{T[]}"/> to initialize the current instance with.</param>
    /// <remarks>
    /// This constructor is used to create a <see cref="QueryResult{T}"/> from an existing <see cref="OperationResult{T[]}"/>. <br/>
    /// It copies the success status, data, and errors from the provided operation result.
    /// </remarks>
    public QueryResult(OperationResult<T[]> operationResult) : base(operationResult)
    {
    }

    /// <summary>
    /// Retrieves the first element of the query result, or the default value if the result is empty.
    /// </summary>
    /// <returns>The first element of the query result or default value if empty.</returns>
    private T? GetFirst()
    {
        if (IsEmpty)
        {
            return default;
        }

        return Data!.FirstOrDefault();
    }

}
