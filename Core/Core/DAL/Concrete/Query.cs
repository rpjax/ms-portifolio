using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a query with parameters that can be used to filter, sort, and paginate data.
/// Implements the <see cref="IQuery{T}"/> interface.
/// </summary>
/// <typeparam name="T">The type of the object being queried.</typeparam>
public class Query<T> : IQuery<T>
{
    /// <summary>
    /// Gets or sets the pagination information for the query.
    /// </summary>
    public PaginationIn Pagination { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter expression for the query.
    /// </summary>
    public Expression? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sort expression for the query.
    /// </summary>
    public Expression? Order { get; set; }

    public Expression? Projection { get; set; }

    public Expression? Aggreration { get; set; }

    /// <summary>
    /// Gets or sets the ordering (ascending or descending) for the query.
    /// </summary>
    public OrderDirection OrderDirection { get; set; } = OrderDirection.Ascending;

    /// <summary>
    /// Initializes an empty query.
    /// </summary>
    public Query()
    {
    }

    /// <summary>
    /// Constructor that initializes the pagination for the query.
    /// </summary>
    /// <param name="pagination">The pagination settings.</param>
    public Query(PaginationIn pagination)
    {
        Pagination = pagination;
    }

    /// <summary>
    /// Constructor that initializes the filter for the query.
    /// </summary>
    /// <param name="lambda">The lambda expression for filtering.</param>
    public Query(Expression<Func<T, bool>> lambda)
    {
        Filter = lambda;
    }

    /// <summary>
    /// Constructor that initializes both the pagination and filter for the query.
    /// </summary>
    /// <param name="pagination">The pagination settings.</param>
    /// <param name="lambda">The lambda expression for filtering.</param>
    public Query(PaginationIn pagination, Expression<Func<T, bool>> lambda) : this(pagination)
    {
        Filter = lambda;
    }

    /// <summary>
    /// Sets the pagination for the query and returns the updated query object.
    /// </summary>
    /// <param name="pagination">The new pagination settings.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> SetPagination(PaginationIn pagination)
    {
        Pagination = pagination;
        return this;
    }

    /// <summary>
    /// Sets the filter for the query and returns the updated query object.
    /// </summary>
    /// <param name="filter">The new filter expression.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> SetFilter(Expression<Func<T, bool>>? filter)
    {
        Filter = filter;
        return this;
    }

    /// <summary>
    /// Sets the sort criteria for the query and returns the updated query object.
    /// </summary>
    /// <param name="sort">The new sort expression.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> SetSort(Expression<Func<T, object>> sort)
    {
        Order = sort;
        return this;
    }

    /// <summary>
    /// Sets the ordering for the query and returns the updated query object.
    /// </summary>
    /// <param name="order">The new order setting.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> SetOrder(OrderDirection order)
    {
        OrderDirection = order;
        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical AND based on the provided expression and 
    /// returns the updated query object.
    /// </summary>
    /// <param name="filter">The expression to be added as an additional filter.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> AndFilter(Expression<Func<T, bool>>? filter)
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

        VisitFilter();
        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical AND based on the provided expression and 
    /// returns the updated query object.
    /// </summary>
    /// <param name="filter">The expression to be added as an additional filter.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> AndFilter(Expression? filter)
    {
        if (filter == null)
        {
            return this;
        }
        if (filter.NodeType == ExpressionType.Lambda)
        {
            throw new ArgumentException(nameof(filter));
        }

        if (Filter == null)
        {
            Filter = Expression.Lambda<Func<T, bool>>(filter, Expression.Parameter(typeof(T)));
        }
        else
        {
            Filter = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(Filter.Body, filter), Filter.Parameters);
        }

        VisitFilter();
        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical OR based on the provided expression and 
    /// returns the updated query object.
    /// </summary>
    /// <param name="filter">The expression to be added as an additional filter.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> OrFilter(Expression<Func<T, bool>>? filter)
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

        VisitFilter();
        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical OR based on the provided expression and 
    /// returns the updated query object.
    /// </summary>
    /// <param name="filter">The expression to be added as an additional filter.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> OrFilter(Expression? filter)
    {
        if (filter == null)
        {
            return this;
        }
        if (filter.NodeType == ExpressionType.Lambda)
        {
            throw new ArgumentException(nameof(filter));
        }

        if (Filter == null)
        {
            Filter = Expression.Lambda<Func<T, bool>>(filter, Expression.Parameter(typeof(T)));
        }
        else
        {
            Filter = Expression.Lambda<Func<T, bool>>(Expression.OrElse(Filter.Body, filter), Filter.Parameters);
        }

        VisitFilter();
        return this;
    }

    /// <summary>
    /// Consolidates and unifies parameter expressions in the filter for seamless combination of multiple filters.
    /// </summary>
    /// <returns>The consolidated filter expression.</returns>
    protected void VisitFilter()
    {
        Filter = new ParameterExpressionConsolidationVisitor()
            .Visit(Filter)
            ?.TypeCast<Expression<Func<T, bool>>>();
    }

    /// <summary>
    /// Internal helper class that visits and consolidates parameter expressions within filter expressions. <br/>
    /// This is used to ensure that the filter expressions 
    /// can be combined seamlessly without conflicting parameter expressions.
    /// </summary>
    internal class ParameterExpressionConsolidationVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Gets the root parameter expression identified during the visitation process.
        /// </summary>
        ParameterExpression? RootParameterExpression { get; set; } = null;

        /// <summary>
        /// Visits and potentially modifies a lambda expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of delegate of the lambda expression.</typeparam>
        /// <param name="node">The lambda expression to visit.</param>
        /// <returns>The modified lambda expression.</returns>
        protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
        {
            if (node.NodeType != ExpressionType.Lambda)
            {
                return base.VisitLambda(node);
            }

            var lambdaExpression = node.TypeCast<LambdaExpression>();

            RootParameterExpression = lambdaExpression.Parameters.First();

            return base.VisitLambda(node);
        }

        /// <summary>
        /// Visits and potentially modifies a member access expression.
        /// </summary>
        /// <param name="node">The member access expression to visit.</param>
        /// <returns>The modified member access expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression?.Type == RootParameterExpression?.Type)
            {
                return Expression.MakeMemberAccess(RootParameterExpression, node.Member);
            }

            return base.VisitMember(node);
        }
    }
}
