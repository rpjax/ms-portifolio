using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Defines the structure for a query that can be used to filter, sort, and paginate data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data the query is designed for.</typeparam>
public interface IQuery<T>
{
    /// <summary>
    /// Gets or sets the pagination information for the query, used to limit the number of records returned.
    /// </summary>
    PaginationIn Pagination { get; set; }

    /// <summary>
    /// Gets or sets the filter expression that should be applied to the data.
    /// </summary>
    /// <remarks>
    /// This is an expression that defines the condition each element of type <typeparamref name="T"/> must satisfy to be included in the result.
    /// </remarks>
    Expression? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sorting expression that should be applied to the data.
    /// </summary>
    /// <remarks>
    /// This is an expression that defines how the elements should be ordered in the result set.
    /// </remarks>
    Expression? Order { get; set; }

    Expression? Projection { get; set; }

    Expression? Aggreration { get; set; }

    /// <summary>
    /// Gets or sets the order in which the data should be sorted.
    /// </summary>
    /// <remarks>
    /// Use <see cref="OrderDirection.Ascending"/> for ascending order and <see cref="OrderDirection.Descending"/> for descending order.
    /// </remarks>
    OrderDirection OrderDirection { get; set; }
}
