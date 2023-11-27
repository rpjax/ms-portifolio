using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

public class OrderExpression
{
    public Expression Expression { get; }
    public OrderDirection Direction { get; }

    public OrderExpression(Expression expression, OrderDirection direction)
    {
        Expression = expression;
        Direction = direction;
    }
}

public static class OrderGenerator
{
    //*
    // { order: { prop: "ascending", prop2: { subProp: "descending" } } }
    //*
    public static IEnumerable<OrderExpression> Translate(GeneratorContext context, Node node)
    {
        if (node is not ScopeDefinitionNode scope)
        {
            throw new GeneratorException("The 'order' node must be a scope definition (object) with ordering expressions.", node);
        }

        foreach (var expression in scope.Expressions)
        {
            if (expression.Rhs.Value is LiteralNode)
            {
                yield return TranslateLiteralExpression(context, expression);
            }

            if (expression.Rhs.Value is ScopeDefinitionNode _scope)
            {
                var memberName = expression.Lhs.Value;
                var subContext = context.CreateSubContext(memberName);

                foreach (var _expression in TranslateScopeExpression(subContext, _scope))
                {
                    yield return _expression;
                }
            }
        }
    }

    private static IEnumerable<OrderExpression> TranslateScopeExpression(GeneratorContext context, ScopeDefinitionNode node)
    {
        foreach (var item in node.Expressions)
        {
            if (item.Rhs.Value is LiteralNode)
            {
                yield return TranslateLiteralExpression(context, item);
                continue;
            }

            if (item.Rhs.Value is ScopeDefinitionNode scope)
            {
                var memberName = item.Lhs.Value;
                var subContext = context.CreateSubContext(memberName);

                foreach (var expression in TranslateScopeExpression(subContext, scope))
                {
                    yield return expression;
                }

                continue;
            }
        }
    }

    private static OrderExpression TranslateLiteralExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not LiteralNode literal)
        {
            throw new GeneratorException($"Ordering Error: The right-hand side of the order expression for '{node.Lhs.Value}' must be a literal indicating the sort direction ('ascending' or 'descending').", node);
        }

        var memberName = node.Lhs.Value;
        var subContext = context.CreateSubContext(memberName);

        return new OrderExpression(subContext.Expression, GetOrderDirection(literal));
    }

    public static string? TranslateStringLiteral(LiteralNode node)
    {
        return node.Value?[1..^1];
    }

    private static OrderDirection GetOrderDirection(LiteralNode node)
    {
        if (TranslateStringLiteral(node) == "ascending")
        {
            return OrderDirection.Ascending;
        }
        if (TranslateStringLiteral(node) == "descending")
        {
            return OrderDirection.Descending;
        }

        throw new GeneratorException($"Invalid order direction: '{node.Value}'. Expected 'ascending' or 'descending'.", node);
    }
}
