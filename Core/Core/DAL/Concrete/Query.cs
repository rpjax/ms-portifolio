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
    public Expression<Func<T, bool>>? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sort expression for the query.
    /// </summary>
    public Expression<Func<T, object>>? Sort { get; set; }

    /// <summary>
    /// Gets or sets the ordering (ascending or descending) for the query.
    /// </summary>
    public Ordering Order { get; set; } = Ordering.Ascending;

    /// <summary>
    /// Default constructor.
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
        Sort = sort;
        return this;
    }

    /// <summary>
    /// Sets the ordering for the query and returns the updated query object.
    /// </summary>
    /// <param name="order">The new order setting.</param>
    /// <returns>The updated query object.</returns>
    public Query<T> SetOrder(Ordering order)
    {
        Order = order;
        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical AND and returns the updated query object.
    /// </summary>
    /// <param name="filter">The additional filter to add.</param>
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
            Filter = VisitFilter();
        }

        return this;
    }

    /// <summary>
    /// Adds additional filter conditions using logical OR and returns the updated query object.
    /// </summary>
    /// <param name="filter">The additional filter to add.</param>
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
            Filter = VisitFilter();
        }

        return this;
    }

    protected Expression<Func<T, bool>>? VisitFilter()
    {
        return new ParameterExpressionConsolidationVisitor()
            .Visit(Filter)
            ?.TypeCast<Expression<Func<T, bool>>>();
    }

    internal class ParameterExpressionConsolidationVisitor : ExpressionVisitor
    {
        ParameterExpression? RootParameterExpression { get; set; } = null;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.NodeType != ExpressionType.Lambda)
            {
                return base.VisitLambda<T>(node);
            }

            var lambdaExpression = node.TypeCast<LambdaExpression>();

            RootParameterExpression = lambdaExpression.Parameters.First();

            return base.VisitLambda(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.Type == RootParameterExpression?.Type)
            {
                return Expression.MakeMemberAccess(RootParameterExpression, node.Member);
            }

            return base.VisitMember(node);
        }
    }
}
