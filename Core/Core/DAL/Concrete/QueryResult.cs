using System.Text.Json.Serialization;

namespace ModularSystem.Core;

public class QueryResult<T> : OperationResult<T[]>, IQueryResult<T>
{
    [JsonIgnore]
    public bool IsEmpty => Data?.IsEmpty() == true;

    [JsonIgnore]
    public bool IsNotEmpty => !IsEmpty;

    /// <inheritdoc/>
    public long Length => Data?.LongLength ?? default;

    /// <inheritdoc/>
    [JsonIgnore]
    public T? First => GetFirst();

    /// <summary>
    /// Default constructor.
    /// </summary>
    [JsonConstructor]
    public QueryResult() 
    {
    }

    public QueryResult(IEnumerable<T> data, params Error[] errors) 
        : base(true, data.ToArray(), errors)
    {
        
    }

    public QueryResult(params Error[] errors) : base(errors)
    {
    }

    public QueryResult(IEnumerable<Error> errors) : base(errors)
    {
    }

    private T? GetFirst()
    {
        if(IsEmpty)
        {
            return default;
        }

        return Data!.FirstOrDefault();
    }

}
