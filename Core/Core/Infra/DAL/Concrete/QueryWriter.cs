using ModularSystem.Core.Expressions;
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

    private ComplexOrderingWriter<T> OrderingWriter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryWriter{T}"/> class, optionally starting with an existing query.
    /// </summary>
    /// <param name="query">An optional initial query to begin with.</param>
    public QueryWriter(IQuery<T>? query = null)
    {
        Query = new();
        OrderingWriter = new();

        if (query != null)
        {
            Query.Filter = query.Filter;
            Query.Ordering = query.Ordering;
            Query.Pagination = query.Pagination;
        }
        if (Query.Ordering is ComplexOrderingExpression complexOrdering)
        {
            OrderingWriter.AddOrdering(complexOrdering);
        }
    }

    /// <summary>
    /// Produces the final <see cref="Query{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The constructed query object.</returns>
    public Query<T> Create()
    {
        return new Query<T>
        {
            Filter = Query.Filter,
            Ordering = OrderingWriter.Create(),
            Pagination = Query.Pagination,
        };
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

// partial dedicated to ordering 
public partial class QueryWriter<T>
{
    /// <summary>
    /// Clears the ordering configuration for the query.
    /// </summary>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> ClearOrdering()
    {
        OrderingWriter.Clear();
        return this;
    }

    /// <summary>
    /// Adds an ordering expression to the query, determining the field or property by which the results should be sorted.
    /// </summary>
    /// <typeparam name="TField">The type of the field or property by which to sort.</typeparam>
    /// <param name="fieldSelector">The ordering expression to set.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> AddOrdering<TField>(Expression<Func<T, TField>> fieldSelector, OrderingDirection direction)
    {
        OrderingWriter.AddOrdering<TField>(fieldSelector, direction);
        return this;
    }

    /// <summary>
    /// Sets the ordering expression for the query, determining the field or property by which the results should be sorted.<br/>
    /// Clears any existing ordering configuration before setting the new ordering expression.
    /// </summary>
    /// <typeparam name="TField">The type of the field or property by which to sort.</typeparam>
    /// <param name="fieldSelector">The ordering expression to set.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    /// <returns>The current instance of the writer.</returns>
    public QueryWriter<T> SetOrdering<TField>(Expression<Func<T, TField>> fieldSelector, OrderingDirection direction)
    {
        ClearOrdering();
        AddOrdering(fieldSelector, direction);
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
