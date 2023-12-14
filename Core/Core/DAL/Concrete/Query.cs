using ModularSystem.Web;
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
    public Expression? Ordering { get; set; }

    /// <inheritdoc/>
    public PaginationIn Pagination { get; set; } = new();

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
        Ordering = query.Ordering;
        Pagination = query.Pagination;
    }

    public Query<TNew> TypeConvert<TNew>()
    {
        return new Query<TNew>
        {
            Filter = Filter,
            Ordering = Ordering,
            Pagination = Pagination,
        };
    }

    /// <summary>
    /// Converts the query into a <see cref="SerializableQuery"/> format using <see cref="QueryProtocol"/>.
    /// </summary>
    /// <returns>A serialized representation of the query.</returns>
    public SerializableQuery ToSerializable()
    {
        return new()
        {
            Filter = QueryProtocol.ToSerializable(Filter),
            Ordering = QueryProtocol.ToSerializable(Ordering),
            Pagination = Pagination
        };
    }

}
