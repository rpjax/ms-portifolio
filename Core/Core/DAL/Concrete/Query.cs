using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a query with parameters that can be used to filter, group, project (select), sort, and paginate data of type <typeparamref name="T"/>.
/// Implements the <see cref="IQuery{T}"/> interface.
/// </summary>
/// <typeparam name="T">The type of the object being queried.</typeparam>
public class Query<T> : IQuery<T>
{
    /// <inheritdoc/>
    public Expression? Filter { get; set; }

    /// <inheritdoc/>
    public Expression? Grouping { get; set; }

    /// <inheritdoc/>
    public Expression? Projection { get; set; }

    /// <inheritdoc/>
    public Expression? Ordering { get; set; }

    /// <inheritdoc/>
    public PaginationIn Pagination { get; set; } = new();

    /// <summary>
    /// Gets or sets the ordering (ascending or descending) for the query.
    /// </summary>
    public OrderingDirection OrderingDirection { get; set; } = OrderingDirection.Ascending;

    /// <summary>
    /// Initializes an empty query.
    /// </summary>
    public Query()
    {
    }

    public Query(IQuery<T> query)
    {
        Filter = query.Filter;
        Grouping = query.Grouping;
        Projection = query.Projection;
        Ordering = query.Ordering;
        Pagination = query.Pagination;
        OrderingDirection = OrderingDirection.Ascending;
    }

    /// <summary>
    /// Constructor that initializes the pagination for the query.
    /// </summary>
    /// <param name="pagination">The pagination settings.</param>
    public Query(PaginationIn pagination)
    {
        Pagination = pagination;
    }

    ///// <summary>
    ///// Constructor that initializes the filter for the query.
    ///// </summary>
    ///// <param name="lambda">The lambda expression for filtering.</param>
    //public Query(Expression<Func<T, bool>> lambda)
    //{
    //    Filter = lambda;
    //}

    ///// <summary>
    ///// Constructor that initializes both the pagination and filter for the query.
    ///// </summary>
    ///// <param name="pagination">The pagination settings.</param>
    ///// <param name="lambda">The lambda expression for filtering.</param>
    //public Query(PaginationIn pagination, Expression<Func<T, bool>> lambda) : this(pagination)
    //{
    //    Filter = lambda;
    //}

}

