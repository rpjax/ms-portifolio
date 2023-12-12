using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;
using System.Linq.Expressions;

namespace ModularSystem.Webql;

/// <summary>
/// Provides parsing functionality for WebQL queries, converting JSON strings into syntax trees, <br/>
/// and transforming these trees into various expressions and structures.
/// </summary>
public class Parser
{
    private ParserOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="options">The parser options.</param>
    public Parser(ParserOptions? options = null)
    {
        Options = options ?? new();
    }

    /// <summary>
    /// Performs a syntax analysis and returns the syntax tree with no further transformations.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ObjectNode Parse(string json)
    {
        try
        {
            return SyntaxAnalyser.Parse(json);
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON expression node to obtain the pagination limit.
    /// </summary>
    /// <param name="node">The JSON expression node containing the pagination limit.</param>
    /// <returns>The parsed pagination limit value.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public long ParseToPaginationLimit(ExpressionNode node)
    {
        try
        {
            var context = new SyntaxContext("$.limit");

            if (node.Rhs.Value is not LiteralNode literal)
            {
                throw new SyntaxException("", context);
            }

            if (literal.Value == null)
            {
                return Options.DefaultLimit;
            }

            if (!long.TryParse(literal.Value, out var value))
            {
                throw new SyntaxException("", context);
            }

            return value;
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    public long ParseToPaginationOffset(ExpressionNode node)
    {
        try
        {
            var context = new SyntaxContext("$.offset");

            if (node.Rhs.Value is not LiteralNode literal)
            {
                throw new SyntaxException("", context);
            }

            if (literal.Value == null)
            {
                return Options.DefaultOffset;
            }

            if (!long.TryParse(literal.Value, out var value))
            {
                throw new SyntaxException("", context);
            }

            return value;
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the root syntax tree to obtain the pagination information.
    /// </summary>
    /// <param name="root">The root syntax tree node.</param>
    /// <returns>The parsed pagination information.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public PaginationIn ParseToPagination(ObjectNode root)
    {
        try
        {
            var limitExpression = root[QueryDefinition.LimitKey];
            var offsetExpression = root[QueryDefinition.OffsetKey];
            var limit = Options.DefaultLimit;
            var offset = Options.DefaultOffset;

            if (limitExpression != null)
            {
                limit = ParseToPaginationLimit(limitExpression);
            }
            if (offsetExpression != null)
            {
                offset = ParseToPaginationOffset(offsetExpression);
            }

            return new(limit, offset);
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON expression node to obtain a filter expression.
    /// </summary>
    /// <param name="node">The JSON expression node containing the filter information.</param>
    /// <param name="type">The type of the entity for filtering.</param>
    /// <returns>The parsed filter expression.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public Expression? ParseToFilter(ExpressionNode node, Type type)
    {
        try
        {
            var syntaxTree = FilterSemanticsAnalyser.Parse(type, node);

            var parameter = Expression.Parameter(type, "x");
            var context = new GeneratorContext(type, parameter);
            var lambdaParamType = context.Type;

            var parameters = new ParameterExpression[] { Expression.Parameter(lambdaParamType, "x") };
            var body = FilterGenerator.Translate(context, syntaxTree);
            var expression = Expression.Lambda(body, parameters);

            var visitor = new ParameterExpressionUniformityVisitor();

            return visitor.Visit(expression);
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON expression node to obtain a filter expression as a lambda function.
    /// </summary>
    /// <typeparam name="T">The type of the entity for filtering.</typeparam>
    /// <param name="node">The JSON expression node containing the filter information.</param>
    /// <returns>The parsed filter expression as a lambda function.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public Expression<Func<T, bool>>? ParseToLambdaFilter<T>(ExpressionNode node)
    {
        try
        {
            var expression = ParseToFilter(node, typeof(T));

            if (expression == null)
            {
                return null;
            }

            return expression.TypeCast<Expression<Func<T, bool>>>();
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON node to obtain ordering definitions.
    /// </summary>
    /// <param name="node">The JSON node containing ordering information.</param>
    /// <param name="type">The type of the entity for ordering.</param>
    /// <returns>The parsed ordering definitions.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public OrderDefinition[] ParseToOrderDefinitions(Node node, Type type)
    {
        try
        {
            var syntaxTree = OrderSemanticsAnalyser.Parse(type, node);
            var parameter = Expression.Parameter(type, "x");
            var context = new GeneratorContext(type, parameter);

            return OrderGenerator
                .Translate(context, syntaxTree)
                .ToArray();
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON node to obtain ordering definitions for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity for ordering.</typeparam>
    /// <param name="node">The JSON node containing ordering information.</param>
    /// <returns>The parsed ordering definitions.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public OrderDefinition[]? ParseToOrderDefinitions<T>(Node node)
    {
        return ParseToOrderDefinitions(node, typeof(T));
    }

    /// <summary>
    /// Parses the specified JSON string to obtain a query definition.
    /// </summary>
    /// <param name="json">The JSON string representing the WebQL query.</param>
    /// <returns>The parsed query definition.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public QueryDefinition ParseToQueryDefinition(string json)
    {
        try
        {
            return new QueryDefinition(this, Parse(json));
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    /// <summary>
    /// Parses the specified JSON string to obtain a query for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity for the query.</typeparam>
    /// <param name="json">The JSON string representing the WebQL query.</param>
    /// <returns>The parsed query for the specified entity type.</returns>
    /// <exception cref="AppException">Thrown when an error occurs during parsing.</exception>
    public IQuery<T> ParseToQuery<T>(string json)
    {
        try
        {
            var type = typeof(T);
            var queryDefinition = ParseToQueryDefinition(json);
            var pagination = queryDefinition.GetPagination();
            var orderDefinitions = queryDefinition.GetOrderDefinitions(type);
            var orderingWriter = new ComplexOrderingWriter<T>();

            foreach (var item in orderDefinitions)
            {
                orderingWriter.AddOrdering(item.FieldType, item.Expression, item.GetOrderingDirection());
            }

            var filter = queryDefinition.GetFilterExpression(type);
            var ordering = orderingWriter.Create();

            return new Query<T>()
            {
                Pagination = pagination,
                Filter = filter,
                Ordering = ordering
            };
        }
        catch (Exception e)
        {
            throw HandleError(e);
        }
    }

    private Exception HandleError(Exception e)
    {
        if (e is ParseException parserException)
        {
            return new AppException(parserException.GetMessage(), ExceptionCode.InvalidInput, e);
        }

        return new AppException("An internal error occurred while processing the WebQL query.", ExceptionCode.Internal, e);
    }

}

/// <summary>
/// Represents the options for the <see cref="Parser"/>.
/// </summary>
public class ParserOptions
{
    /// <summary>
    /// Gets or sets the default limit for pagination.
    /// </summary>
    public long DefaultLimit { get; set; } = PaginationIn.DefaultLimit;

    /// <summary>
    /// Gets or sets the default offset for pagination.
    /// </summary>
    public long DefaultOffset { get; set; } = PaginationIn.DefaultOffset;
}
