using System.Linq.Expressions;

namespace ModularSystem.Core;

// partial dedicated to commom components.
/// <summary>
/// Provides a factory for building and refining <see cref="IQuery{T}"/> objects for entities of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This factory is designed to be used in a fluent manner. The query creation and refinement methods return the factory itself, allowing for chaining of modifications.
/// </remarks>
public partial class QueryWriter<T> : IFactory<IQuery<T>>
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
    /// Produces the final <see cref="IQuery{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The constructed query object.</returns>
    public IQuery<T> Create()
    {
        return Query;
    }

    /// <summary>
    /// Constructs an expression visitor to ensure uniformity in parameter references across combined expressions.
    /// </summary>
    /// <returns>A newly created expression visitor.</returns>
    protected ExpressionVisitor CreateExpressionVisitor()
    {
        return new ParameterExpressionUniformityVisitor();
    }

    /// <summary>
    /// Visits and potentially modifies an expression.
    /// </summary>
    /// <typeparam name="TResult">Type of expression to be visited.</typeparam>
    /// <param name="expression">Expression to visit.</param>
    /// <returns>Modified expression, if any; otherwise the original expression.</returns>
    protected TResult? VisitExpression<TResult>(TResult? expression) where TResult : Expression
    {
        if (expression == null)
        {
            return null;
        }

        return CreateExpressionVisitor().Visit(expression).TypeCast<TResult>();
    }

    //*
    // visiting shortcuts to normalize expressions when a modification is performed
    //*

    /// <summary>
    /// Visits and potentially modifies the filter expression of the current query.
    /// </summary>
    protected void VisitFilter()
    {
        Query.Filter = VisitExpression(Query.Filter);
    }

    /// <summary>
    /// Visits and potentially modifies the grouping expression of the current query.
    /// </summary>
    protected void VisitGrouping()
    {
        Query.Grouping = VisitExpression(Query.Grouping);
    }

    /// <summary>
    /// Visits and potentially modifies the projection expression of the current query.
    /// </summary>
    protected void VisitProjcetion()
    {
        Query.Projection = VisitExpression(Query.Projection);
    }

    /// <summary>
    /// Visits and potentially modifies the ordering expression of the current query.
    /// </summary>
    protected void VisitOrdering()
    {
        Query.Ordering = VisitExpression(Query.Ordering);
    }
}

// partial dedicated to filter modification
public partial class QueryWriter<T>
{
    public QueryWriter<T> SetFilter(Expression<Func<T, bool>>? filter)
    {
        Query.Filter = filter;
        return this;
    }

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

        VisitFilter();
        return this;
    }

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

        VisitFilter();
        return this;
    }

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

        VisitFilter();
        return this;
    }

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

        VisitFilter();
        return this;
    }

}

// partial dedicated to grouping modification
public partial class QueryWriter<T>
{
    // TODO...
}

// partial dedicated to projection modification
public partial class QueryWriter<T>
{
    public QueryWriter<T> SetProjection<TProjection>(Expression<Func<T, TProjection>> expression)
    {
        Query.Projection = expression;
        VisitProjcetion();
        return this;
    }
}

// partial dedicated to ordering modification
public partial class QueryWriter<T>
{
    public QueryWriter<T> SetOrdering<TField>(Expression<Func<T, TField>> sort)
    {
        Query.Ordering = sort;
        return this;
    }

    public QueryWriter<T> SetOrderDirection(OrderingDirection order)
    {
        Query.OrderingDirection = order;
        return this;
    }
}

// partial dedicated to pagination modification
public partial class QueryWriter<T>
{
    public QueryWriter<T> SetPagination(PaginationIn pagination)
    {
        Query.Pagination = pagination;
        return this;
    }

    public QueryWriter<T> SetLimit(long limit)
    {
        Query.Pagination.Limit = limit;
        return this;
    }

    public QueryWriter<T> SetOffset(long limit)
    {
        Query.Pagination.Limit = limit;
        return this;
    }

    public QueryWriter<T> SetPaginationState(object? state)
    {
        Query.Pagination.State = state;
        return this;
    }

}
