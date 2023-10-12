using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

// partial dedicated to commom components.

/// <summary>
/// Serves as an interpretative tool for extracting and analyzing the expressions and components within a <see cref="Query{T}"/> data structure. <br/>
/// This class provides a specific way to interpret the query, its design allows for potential alternative interpretations in future implementations or extensions.
/// </summary>
/// <typeparam name="T">The type of the entity being targeted by the query.</typeparam>
public partial class QueryReader<T>
{
    private Query<T> Query { get; }
    private Configs Config { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryReader{T}"/> class using a specific query and optional configurations.
    /// </summary>
    /// <param name="query">The query to read from.</param>
    /// <param name="configs">Optional configurations for the reader.</param>
    public QueryReader(Query<T> query, Configs? configs = null)
    {
        Query = query;
        Config = configs ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryReader{T}"/> class using an interface representation of a query and optional configurations.
    /// </summary>
    /// <param name="query">The query to read from.</param>
    /// <param name="configs">Optional configurations for the reader.</param>
    public QueryReader(IQuery<T> query, Configs? configs = null)
    {
        Query = new(query);
        Config = configs ?? new();
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
    /// Visits and potentially modifies an expression to ensure consistency.
    /// </summary>
    /// <typeparam name="TResult">Type of expression to be visited.</typeparam>
    /// <param name="expression">Expression to visit.</param>
    /// <returns>Modified expression, if any; otherwise the original expression.</returns>
    [return: NotNullIfNotNull("expression")]
    protected TResult? VisitExpression<TResult>(TResult? expression) where TResult : Expression
    {
        if (expression == null)
        {
            return null;
        }

        return CreateExpressionVisitor().Visit(expression).TypeCast<TResult>();
    }

    /// <summary>
    /// Configuration settings for the <see cref="QueryReader{T}"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use the ParameterUniformityVisitor for ensuring consistent parameter references.
        /// </summary>
        public bool UseParameterUniformityVisitor { get; set; } = true;
    }
}

// partial dedicated to filter.
public partial class QueryReader<T> 
{
    /// <summary>
    /// Retrieves the filter expression from the query. <br/>
    /// If the configuration is set to use the ParameterUniformityVisitor, the expression is visited to ensure consistent parameter references.
    /// </summary>
    /// <returns>The filter expression, potentially modified for parameter uniformity; otherwise the original filter expression.</returns>
    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        if (Config.UseParameterUniformityVisitor)
        {
            return VisitExpression(Query.Filter as Expression<Func<T, bool>>);
        }

        return Query.Filter as Expression<Func<T, bool>>;
    }
}

// partial dedicated to grouping.
public partial class QueryReader<T>
{
    // TODO...
}

// partial dedicated to projection.
public partial class QueryReader<T>
{
    // TODO...
}

// partial dedicated to ordering.
public partial class QueryReader<T>
{
    /// <summary>
    /// Retrieves the ordering expression from the query. <br/>
    /// If the configuration is set to use the ParameterUniformityVisitor, <br/>
    /// the field selector of the ordering expression is visited to ensure consistent parameter references.
    /// </summary>
    /// <returns>The ordering expression, potentially with a modified field selector for parameter uniformity; otherwise the original ordering expression.</returns>
    public OrderingExpression? GetOrderingExpression()
    {
        if (Config.UseParameterUniformityVisitor)
        {
            if (Query.Ordering is OrderingExpression cast)
            {
                return new(cast.FieldType, VisitExpression(cast.FieldSelector));
            }
        }

        return Query.Ordering as OrderingExpression;
    }

    /// <summary>
    /// Retrieves the field selector expression from the ordering expression.
    /// </summary>
    /// <typeparam name="TField">The type of the field being ordered.</typeparam>
    /// <returns>The field selector expression used for ordering, if available; otherwise null.</returns>
    public Expression<Func<T, TField>>? GetOrderingSelectorExpression<TField>()
    {
        return GetOrderingExpression()?.FieldSelector as Expression<Func<T, TField>>;
    }

    /// <summary>
    /// Retrieves the direction of ordering specified in the query.
    /// </summary>
    /// <returns>The direction of ordering.</returns>
    public OrderingDirection GetOrderingDirection()
    {
        return Query.OrderingDirection;
    }
}

// partial dedicated to pagination.
public partial class QueryReader<T>
{
    /// <summary>
    /// Gets the limit for the number of records to retrieve as a long.
    /// </summary>
    public long Limit => GetLimit();

    /// <summary>
    /// Gets the offset for the starting point of records retrieval as a long.
    /// </summary>
    public long Offset => GetOffset();

    /// <summary>
    /// Gets the limit for the number of records to retrieve as an integer.
    /// </summary>
    public int IntLimit => GetIntLimit();

    /// <summary>
    /// Gets the offset for the starting point of records retrieval as an integer.
    /// </summary>
    public int IntOffset => GetIntOffset();

    /// <summary>
    /// Gets the pagination details for the query.
    /// </summary>
    public PaginationIn Pagination => GetPagination();

    /// <summary>
    /// Retrieves the limit for the number of records to retrieve from the query.
    /// </summary>
    /// <returns>The limit as a long.</returns>
    public long GetLimit()
    {
        return Query.Pagination.Limit;
    }

    /// <summary>
    /// Retrieves the offset for the starting point of records retrieval from the query.
    /// </summary>
    /// <returns>The offset as a long.</returns>
    public long GetOffset()
    {
        return Query.Pagination.Offset;
    }

    /// <summary>
    /// Retrieves the limit for the number of records to retrieve from the query.
    /// </summary>
    /// <returns>The limit as an integer.</returns>
    public int GetIntLimit()
    {
        return Query.Pagination.IntLimit;
    }

    /// <summary>
    /// Retrieves the offset for the starting point of records retrieval from the query.
    /// </summary>
    /// <returns>The offset as an integer.</returns>
    public int GetIntOffset()
    {
        return Query.Pagination.IntOffset;
    }

    /// <summary>
    /// Retrieves the pagination details from the query.
    /// </summary>
    /// <returns>The pagination details.</returns>
    public PaginationIn GetPagination()
    {
        return Query.Pagination;
    }
}
