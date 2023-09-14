namespace ModularSystem.Core;

/// <summary>
/// Represents the result metadata for paginated queries, including offset, limit, and total count.
/// </summary>
public class PaginationOut
{
    /// <summary>
    /// Gets or sets the offset where the returned records start. This is the number of records to skip before starting to return records.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the limit on the number of records to be returned.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the total number of records that match the query, disregarding limit and offset.
    /// </summary>
    public long Total { get; set; }
}
