using ModularSystem.Core;
using ModularSystem.Web.Expressions;
using System.Linq.Expressions;

namespace ModularSystem.Web;

/// <summary>
/// Represents a serializable and web encodable version of the <see cref="Query{T}"/> class.
/// This class provides mechanisms to convert web-friendly serialized query data into actual expressions and vice versa.
/// </summary>
/// <remarks>
/// The purpose of this class is to facilitate the transfer of complex query data (like filtering and sorting expressions) over the web by serializing the query data into strings.
/// This serialized form can then be deserialized back to actual query expressions when needed.
/// The <see cref="ExpressionSerializer"/> is used to handle the serialization and deserialization of expressions.
/// </remarks>
[Serializable]
public class SerializedQuery
{
    /// <summary>
    /// Gets or sets the pagination information for the query.
    /// </summary>
    public PaginationIn Pagination { get; set; } = new PaginationIn();

    /// <summary>
    /// Gets or sets the serialized filter expression for the query.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the serialized sort expression for the query.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the order direction (ascending or descending) for the query.
    /// </summary>
    public Ordering Order { get; set; } = Ordering.Ascending;

    /// <summary>
    /// Converts the serialized filter string back to its corresponding expression.
    /// </summary>
    /// <typeparam name="T">The type of the entity the filter applies to.</typeparam>
    /// <returns>The filter expression or null if no filter string is set.</returns>
    public Expression<Func<T, bool>>? GetFilterExpression<T>()
    {
        if (Filter == null)
        {
            return null;
        }

        var node = SearchEngine.ExpressionSerializer.FromJson(Filter);
        var expression = SearchEngine.ExpressionSerializer.Deserialize(node);

        return SearchEngine.ExpressionSerializer.ToLambdaExpression<Func<T, bool>>(expression);
    }

    /// <summary>
    /// Converts the serialized sort string back to its corresponding expression.
    /// </summary>
    /// <typeparam name="T">The type of the entity the sort applies to, which must be a class.</typeparam>
    /// <returns>The sort expression or null if no sort string is set.</returns>
    public Expression<Func<T, object>>? GetSortExpression<T>() where T : class
    {
        if (Sort == null)
        {
            return null;
        }

        var node = SearchEngine.ExpressionSerializer.FromJson(Sort);
        var expression = SearchEngine.ExpressionSerializer.Deserialize(node);

        return SearchEngine.ExpressionSerializer.ToLambdaExpression<Func<T, object>>(expression);
    }

    /// <summary>
    /// Converts the <see cref="SerializedQuery"/> instance into a <see cref="Query{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the entity the query applies to, which must be a class.</typeparam>
    /// <returns>A <see cref="Query{T}"/> instance with the properties set according to this serialized query.</returns>
    public Query<T> ToQuery<T>() where T : class
    {
        return new Query<T>()
        {
            Pagination = Pagination,
            Filter = GetFilterExpression<T>(),
            Sort = GetSortExpression<T>(),
            Order = Order,
        };
    }
}

/// <summary>
/// A factory class responsible for creating and managing instances of <see cref="SerializedQuery"/> for a given entity type.
/// </summary>
/// <typeparam name="T">The type of entity for which the serialized query is being constructed.</typeparam>
/// <remarks>
/// This factory facilitates the construction of serialized queries by allowing you to set pagination, filtering, sorting, and ordering parameters. 
/// It provides a fluent API to configure these parameters and build the serialized query object.
/// </remarks>
public class SerializedQueryFactory<T>
{
    /// <summary>
    /// Gets or sets the pagination settings for the query.
    /// </summary>
    public PaginationIn? Pagination { get; set; }

    /// <summary>
    /// Gets or sets the filtering expression for the query.
    /// </summary>
    public Expression<Func<T, bool>>? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sorting expression for the query.
    /// </summary>
    public Expression<Func<T, object>>? Sort { get; set; }

    /// <summary>
    /// Gets or sets the ordering direction (ascending or descending) for the query.
    /// </summary>
    public Ordering Ordering { get; set; } = Ordering.Ascending;

    /// <summary>
    /// Constructs a <see cref="SerializedQuery"/> instance based on the current factory settings.
    /// </summary>
    /// <returns>A new <see cref="SerializedQuery"/> object configured with the provided settings.</returns>
    public SerializedQuery Create()
    {
        return new SerializedQuery()
        {
            Pagination = Pagination ?? new(),
            Filter = Filter != null ? SearchEngine.ExpressionSerializer.ToJson(Filter) : null,
            Sort = Sort != null ? SearchEngine.ExpressionSerializer.ToJson(Sort) : null,
            Order = Ordering,
        };
    }

    public SerializedQueryFactory<T> SetPagination(PaginationIn? pagination)
    {
        if (pagination == null)
        {
            return this;
        }

        Pagination = pagination;
        return this;
    }

    public SerializedQueryFactory<T> SetFilter(Expression<Func<T, bool>>? filter)
    {
        if (filter == null)
        {
            return this;
        }

        Filter = filter;
        return this;
    }

    public SerializedQueryFactory<T> SetSort(Expression<Func<T, object>>? sort)
    {
        if (sort == null)
        {
            return this;
        }

        Sort = sort;
        return this;
    }

    public SerializedQueryFactory<T> SetOrder(Ordering order)
    {
        Ordering = order;
        return this;
    }

    /// <summary>
    /// Adds an additional filter to the existing filter expression using the logical AND operation.
    /// </summary>
    /// <param name="filter">The filtering expression to be added.</param>
    /// <returns>The current factory instance for continued configuration.</returns>
    public SerializedQueryFactory<T> AndFilter(Expression<Func<T, bool>>? filter)
    {
        if (filter == null)
        {
            return this;
        }

        if (Filter == null)
        {
            Filter = filter;
        }
        else
        {
            Filter = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(Filter.Body, filter.Body), Filter.Parameters);
        }

        return this;
    }

    /// <summary>
    /// Adds an additional filter to the existing filter expression using the logical OR operation.
    /// </summary>
    /// <param name="filter">The filtering expression to be added.</param>
    /// <returns>The current factory instance for continued configuration.</returns>
    public SerializedQueryFactory<T> OrFilter(Expression<Func<T, bool>>? filter)
    {
        if (filter == null)
        {
            return this;
        }

        if (Filter == null)
        {
            Filter = filter;
        }
        else
        {
            Filter = Expression.Lambda<Func<T, bool>>(Expression.OrElse(Filter.Body, filter.Body), Filter.Parameters);
        }

        return this;
    }
}
