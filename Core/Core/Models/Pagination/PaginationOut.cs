namespace ModularSystem.Core;

/// <summary>
/// Represents the result metadata for paginated queries. This metadata includes information <br/>
/// such as the starting point of the returned records (offset), the number of records to return (limit), <br/>
/// and the total number of records that match the query criteria (total count).
/// </summary>
public class PaginationOut
{
    /// <summary>
    /// Gets or sets the offset where the returned records start. This is the number of records to skip before starting to return records.
    /// </summary>
    public long Offset { get; set; }

    /// <summary>
    /// Gets or sets the limit on the number of records to be returned. This is the maximum number of records to be included in the results.
    /// </summary>
    public long Limit { get; set; }

    /// <summary>
    /// Gets or sets the total number of records that match the query criteria, irrespective of the applied limit and offset. <br/>
    /// This allows users to understand the total size of the dataset that matches their criteria.
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// This field is reserved for mechanisms of pagination that rely on a state preservation to optimize operations.<br/>
    /// It can be used for strategies like token/cursor-based pagination to keep track of the state.
    /// </summary>
    public string? EncodedState { get; set; }

    /// <summary>
    /// Provides an integer representation of the Limit property.
    /// This is useful for systems or contexts where integer-based limits are preferable or required.
    /// </summary>
    public int IntLimit => (int)Limit;

    /// <summary>
    /// Provides an integer representation of the Offset property.
    /// This is useful for systems or contexts where integer-based offsets are preferable or required.
    /// </summary>
    public int IntOffset => (int)Offset;

    /// <summary>
    /// Provides an integer representation of the Total property.
    /// This is useful for systems or contexts where integer-based total counts are preferable or required.
    /// </summary>
    public int IntTotal => (int)Total;
}
