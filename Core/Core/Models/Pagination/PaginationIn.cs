namespace ModularSystem.Core;

/// <summary>
/// Represents pagination parameters with configurable limit and offset.
/// </summary>
public class PaginationIn
{
    /// <summary>
    /// Default value for the pagination limit when using integer types.
    /// </summary>
    public const int DEFAULT_LIMIT = 30;

    /// <summary>
    /// Default value for the pagination offset when using integer types.
    /// </summary>
    public const int DEFAULT_OFFSET = 0;

    /// <summary>
    /// Default value for the pagination limit when using long integer types.
    /// </summary>
    public const long LONG_DEFAULT_LIMIT = 30L;

    /// <summary>
    /// Default value for the pagination offset when using long integer types.
    /// </summary>
    public const long LONG_DEFAULT_OFFSET = 0L;

    /// <summary>
    /// Gets or sets the limit for the pagination.
    /// </summary>
    public int Limit { get; set; } = DEFAULT_LIMIT;

    /// <summary>
    /// Gets or sets the offset for the pagination.
    /// </summary>
    public int Offset { get; set; } = DEFAULT_OFFSET;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with default limit and offset.
    /// </summary>
    public PaginationIn()
    {
        Limit = DEFAULT_LIMIT;
        Offset = DEFAULT_OFFSET;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with a specified limit and default offset.
    /// </summary>
    /// <param name="limit">The limit for the pagination.</param>
    public PaginationIn(int limit)
    {
        Limit = limit;
        Offset = DEFAULT_OFFSET;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationIn"/> class with specified limit and offset.
    /// </summary>
    /// <param name="limit">The limit for the pagination.</param>
    /// <param name="offset">The offset for the pagination.</param>
    public PaginationIn(int limit, int offset)
    {
        Limit = limit;
        Offset = offset;
    }
}
