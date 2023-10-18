using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents pagination parameters with configurable limit and offset.<br/>
/// This class allows users to specify the number of records to retrieve (limit) and where to start retrieving from (offset).<br/>
/// An optional EncodedState property is available for stateful pagination mechanisms.
/// </summary>
public class PaginationIn
{
    /// <summary>
    /// Default value for the pagination limit when using long integer types.
    /// </summary>
    public const long DefaultLimit = 30;

    /// <summary>
    /// Default value for the pagination offset when using long integer types.
    /// </summary>
    public const long DefaultOffset = 0;

    /// <summary>
    /// Gets or sets the limit for the pagination. 
    /// The limit determines the maximum number of records to retrieve.
    /// </summary>
    public long Limit { get; set; } = DefaultLimit;

    /// <summary>
    /// Gets or sets the offset for the pagination.
    /// The offset determines where to start retrieving records from.
    /// </summary>
    public long Offset { get; set; } = DefaultOffset;

    /// <summary>
    /// This field is reserved for mechanisms of pagination that rely on a state preservation to optimize operations.<br/>
    /// It can be used for strategies like token/cursor-based pagination to keep track of the state.
    /// </summary>
    public object? State { get; set; }

    /// <summary>
    /// Provides an integer representation of the Limit property.
    /// Useful for systems that require integer-based limits.
    /// </summary>
    [JsonIgnore]
    public int IntLimit => (int)Limit;

    /// <summary>
    /// Provides an integer representation of the Offset property.
    /// Useful for systems that require integer-based offsets.
    /// </summary>
    [JsonIgnore]
    public int IntOffset => (int)Offset;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with default limit and offset.
    /// </summary>
    public PaginationIn()
    {
        Limit = DefaultLimit;
        Offset = DefaultOffset;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with a specified limit and default offset.
    /// </summary>
    /// <param name="limit">The limit for the pagination.</param>
    public PaginationIn(long limit)
    {
        Limit = limit;
        Offset = DefaultOffset;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with specified limit and offset.
    /// </summary>
    /// <param name="limit">The limit for the pagination.</param>
    /// <param name="offset">The offset for the pagination.</param>
    public PaginationIn(long limit, long offset)
    {
        Limit = limit;
        Offset = offset;
    }
}
