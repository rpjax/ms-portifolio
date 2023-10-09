using ModularSystem.Web;
using ModularSystem.Web.Expressions;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Query{T}"/> class using another query.
    /// </summary>
    /// <param name="query">The query to initialize from.</param>
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
    /// Serializes the query into a <see cref="SerializedQuery"/> format.
    /// </summary>
    /// <param name="serializer">An optional serializer to use for the serialization. If not provided, the default serializer will be used.</param>
    /// <returns>A serialized representation of the query.</returns>
    public SerializedQuery Serialize(ExpressionSerializer? serializer = null)
    {
        return new()
        {
            Filter = QueryProtocol.ToJson(Filter, serializer),
            Grouping = QueryProtocol.ToJson(Grouping, serializer),
            Projection = QueryProtocol.ToJson(Projection, serializer),
            Ordering = QueryProtocol.ToJson(Ordering, serializer),
            OrderingDirection = OrderingDirection
        };
    }

}
