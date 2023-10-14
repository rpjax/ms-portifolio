using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using System.Linq.Expressions;
using System.Text.Json;

namespace ModularSystem.Core;

// partial dedicated to commom components.
/// <summary>
/// Provides a factory for building and refining <see cref="Query{T}"/> objects for entities of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This factory is designed to be used in a fluent manner. <br/>
/// The query creation and refinement methods return the factory itself, allowing for chaining of modifications.
/// </remarks>
public partial class QueryWriter<T> : IFactory<Query<T>>
{
    /// <summary>
    /// Gets the current state of the query being built by this factory.
    /// </summary>
    private Query<T> Query { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryWriter{T}"/> class, optionally starting with an existing query.
    /// </summary>
    /// <param name="query">An optional initial query to begin with.</param>
    public QueryWriter(IQuery<T>? query = null)
    {
        Query = new();

        if (query != null)
        {
            Query.Filter = query.Filter;
            Query.Grouping = query.Grouping;
            Query.Projection = query.Projection;
            Query.Ordering = query.Ordering;
            Query.Pagination = query.Pagination;
        }
    }

    /// <summary>
    /// Produces the final <see cref="Query{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The constructed query object.</returns>
    public Query<T> Create()
    {
        return Query;
    }

    /// <summary>
    /// Produces a serializable representation of the <see cref="Query{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The serialized representation of the constructed query object.</returns>
    public SerializableQuery CreateSerializable()
    {
        return Query.ToSerializable();
    }

    /// <summary>
    /// Produces a <see cref="string"/> representation of the <see cref="Query{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The serialized string representation of the constructed query object.</returns>
    public string ToJson(JsonSerializerOptions? options = null)
    {
        var serializer = new ExprJsonSerializer(options);
        return serializer.Serialize(CreateSerializable());
    }
}

// partial dedicated to filter 
public partial class QueryWriter<T>
{
    /// <summary>
    /// Sets the filter expression for the query.
    /// If a filter expression already exists, it will be replaced by the provided one.
    /// </summary>
    /// <param name="filter">The filter expression to set or override.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetFilter(Expression<Func<T, bool>>? filter)
    {
        Query.Filter = filter;
        return this;
    }

    /// <summary>
    /// Appends an additional filter to the existing filter using logical AND.
    /// </summary>
    /// <param name="filter">The filter expression to append.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> AndFilter(Expression<Func<T, bool>>? filter)
    {
        if (filter == null)
        {
            return this;
        }

        var reader = new QueryReader<T>(Query);
        var predicate = reader.GetFilterExpression();

        if (predicate == null)
        {
            Query.Filter = filter;
        }
        else
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, filter.Body), predicate.Parameters);
        }

        return this;
    }

    /// <summary>
    /// Appends an additional filter to the existing filter using logical AND.
    /// </summary>
    /// <param name="filter">The filter expression to append.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> AndFilter(Expression? filter)
    {
        if (filter == null)
        {
            return this;
        }
        if (filter.NodeType == ExpressionType.Lambda)
        {
            throw new ArgumentException(nameof(filter));
        }

        var reader = new QueryReader<T>(Query);
        var predicate = reader.GetFilterExpression();

        if (predicate == null)
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(filter, Expression.Parameter(typeof(T)));
        }
        else
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, filter), predicate.Parameters);
        }

        return this;
    }

    /// <summary>
    /// Appends an additional filter to the existing filter using logical OR.
    /// </summary>
    /// <param name="filter">The filter expression to append.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> OrFilter(Expression<Func<T, bool>>? filter)
    {
        if (filter == null)
        {
            return this;
        }

        var reader = new QueryReader<T>(Query);
        var predicate = reader.GetFilterExpression();

        if (predicate == null)
        {
            Query.Filter = filter;
        }
        else
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(Expression.OrElse(predicate.Body, filter.Body), predicate.Parameters);
        }

        return this;
    }

    /// <summary>
    /// Appends an additional filter to the existing filter using logical OR.
    /// </summary>
    /// <param name="filter">The filter expression to append.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> OrFilter(Expression? filter)
    {
        if (filter == null)
        {
            return this;
        }
        if (filter.NodeType == ExpressionType.Lambda)
        {
            throw new ArgumentException(nameof(filter));
        }

        var reader = new QueryReader<T>(Query);
        var predicate = reader.GetFilterExpression();

        if (predicate == null)
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(filter, Expression.Parameter(typeof(T)));
        }
        else
        {
            Query.Filter = Expression.Lambda<Func<T, bool>>(Expression.OrElse(predicate.Body, filter), predicate.Parameters);
        }

        return this;
    }

}

// partial dedicated to grouping 
public partial class QueryWriter<T>
{
    // TODO...
}

// partial dedicated to projection 
public partial class QueryWriter<T>
{
    /// <summary>
    /// Sets the projection expression for the query, determining which fields or properties of the entity should be included in the result set.
    /// </summary>
    /// <typeparam name="TProjection">The type of the projection result.</typeparam>
    /// <param name="expression">The projection expression to set.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetProjection<TProjection>(Expression<Func<T, TProjection>> expression)
    {
        Query.Projection = expression;
        return this;
    }
}

// partial dedicated to ordering 
public partial class QueryWriter<T>
{
    /// <summary>
    /// Sets the ordering expression for the query, determining the field or property by which the results should be sorted.
    /// </summary>
    /// <typeparam name="TField">The type of the field or property by which to sort.</typeparam>
    /// <param name="sort">The ordering expression to set.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetOrdering<TField>(Expression<Func<T, TField>> sort)
    {
        Query.Ordering = sort;
        return this;
    }

    /// <summary>
    /// Sets the direction in which the results should be ordered, either ascending or descending.
    /// </summary>
    /// <param name="order">The ordering direction to set.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetOrderDirection(OrderingDirection order)
    {
        Query.OrderingDirection = order;
        return this;
    }
}

// partial dedicated to pagination 
public partial class QueryWriter<T>
{
    /// <summary>
    /// Sets the pagination details for the query, determining how results should be paginated.
    /// </summary>
    /// <param name="pagination">The pagination details to set.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetPagination(PaginationIn pagination)
    {
        Query.Pagination = pagination;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of results to return for the query.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetLimit(long limit)
    {
        Query.Pagination.Limit = limit;
        return this;
    }

    /// <summary>
    /// Sets the starting point from which to return results for the query.
    /// </summary>
    /// <param name="offset">The starting point for results.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetOffset(long offset)
    {
        Query.Pagination.Offset = offset;
        return this;
    }

    /// <summary>
    /// Sets the state information for pagination. This is particularly useful for mechanisms of pagination <br/>
    /// that rely on state preservation to optimize operations, such as token or cursor-based pagination.
    /// </summary>
    /// <param name="state">The state information to be used for optimized pagination strategies.</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetPaginationState(object? state)
    {
        Query.Pagination.State = state;
        return this;
    }

}
