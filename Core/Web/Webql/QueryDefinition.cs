using ModularSystem.Core;
using ModularSystem.Webql.Synthesis;
using System.Linq.Expressions;

namespace ModularSystem.Webql;

public class QueryDefinition
{
    public const string LimitKey = "limit";
    public const string OffsetKey = "offset";
    public const string FilterKey = "where";
    public const string OrderKey = "order";

    private Parser Parser { get; }
    private ObjectNode Root { get; }
    private ExpressionNode? LimitSyntaxTree { get; }
    private ExpressionNode? OffetSyntaxTree { get; }
    private ExpressionNode? FilterSyntaxTree { get; }
    private ExpressionNode? OrderSyntaxTree { get; }

    public QueryDefinition(Parser parser, ObjectNode root)
    {
        Parser = parser;
        Root = root;
        LimitSyntaxTree = root[LimitKey];
        OffetSyntaxTree = root[OffsetKey];
        FilterSyntaxTree = root[FilterKey];
        OrderSyntaxTree = root[OrderKey];
    }

    public PaginationIn GetPagination()
    {
        return Parser.ParseToPagination(Root);
    }

    public Expression? GetFilterExpression(Type type)
    {
        if (FilterSyntaxTree == null)
        {
            return null;
        }

        return Parser.ParseToFilter(FilterSyntaxTree, type);
    }

    public Expression? GetFilterExpression<T>()
    {
        return GetFilterExpression(typeof(T));
    }

    public Expression<Func<T, bool>>? GetFilterLambdaExpression<T>()
    {
        if (FilterSyntaxTree == null)
        {
            return null;
        }

        return Parser.ParseToLambdaFilter<T>(FilterSyntaxTree);
    }

    public OrderDefinition[] GetOrderDefinitions(Type type)
    {
        if (OrderSyntaxTree == null)
        {
            return Array.Empty<OrderDefinition>();
        }

        return Parser.ParseToOrderDefinitions(OrderSyntaxTree, type);
    }
}
