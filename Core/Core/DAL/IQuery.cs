using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines the structure for a query to filter, group, project (select), sort, and paginate data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data the query is designed for.</typeparam>
public interface IQuery<T>
{
    /// <summary>
    /// Gets or sets the filter expression that should be applied to the data.
    /// </summary>
    /// <remarks>
    /// Filter the data. This expression defines the conditions each element of type <typeparamref name="T"/> must satisfy to be included in the result.
    /// </remarks>
    Expression? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sorting expression that should be applied to the data.
    /// </summary>
    /// <remarks>
    /// Sort the data. This expression defines how the elements are ordered in the result set after they've been filtered, grouped, and projected.
    /// </remarks>
    Expression? Ordering { get; set; }

    /// <summary>
    /// Gets or sets the pagination information for the query, used to limit the number of records returned.
    /// </summary>
    /// <remarks>
    /// Paginate the results. Determines which subset of the result set is returned based on the defined start point and page size.
    /// </remarks>
    PaginationIn Pagination { get; set; }
}