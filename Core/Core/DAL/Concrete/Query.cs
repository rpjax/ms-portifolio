using ModularSystem.Web;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a query with parameters that can be used to filter, order, and paginate data of type <typeparamref name="T"/>. <br/>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Query{T}"/> class with specified pagination details.
    /// </summary>
    /// <param name="pagination">The pagination details to be applied to the query.</param>
    /// <remarks>
    /// This constructor creates a query that includes only pagination settings, without any filtering or ordering. <br/> 
    /// It is useful when you need to fetch a specific page of results without applying any filters or sorting order.
    /// </remarks>
    public Query(PaginationIn pagination)
    {
        Filter = null;
        Ordering = null;
        Pagination = pagination;
    }

    /// <summary>
    /// Converts the current query to a query of a different type.
    /// </summary>
    /// <typeparam name="TConverted">The new type to which the query is being converted.</typeparam>
    /// <returns>A new <see cref="Query{TNew}"/> instance with the same configuration as the current query, but typed for <typeparamref name="TConverted"/>.</returns>
    /// <remarks>
    /// This method is useful when needing to change the type context of a query, 
    /// while preserving the existing filter, ordering, and pagination settings. 
    /// It creates a new <see cref="Query{TNew}"/> instance and copies the settings from the current query. 
    /// Note that type conversion does not transform the data; it merely changes the type parameter of the query.
    /// </remarks>
    public Query<TConverted> TypeConvert<TConverted>()
    {
        return new Query<TConverted>
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
