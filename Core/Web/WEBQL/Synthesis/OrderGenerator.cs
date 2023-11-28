using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents a definition for ordering in a query.
/// </summary>
public class OrderDefinition
{
    /// <summary>
    /// Gets the type of the field used for ordering.
    /// </summary>
    public Type FieldType { get; }

    /// <summary>
    /// Gets the expression that selects the field from the data source for ordering.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// Gets the direction in which the results should be ordered (ascending or descending).
    /// </summary>
    public OrderDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderDefinition"/> class.
    /// </summary>
    /// <param name="fieldType">The type of the field used for ordering.</param>
    /// <param name="expression">The expression that selects the field from the data source for ordering.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    public OrderDefinition(Type fieldType, Expression expression, OrderDirection direction)
    {
        FieldType = fieldType;
        Expression = expression;
        Direction = direction;
    }

    /// <summary>
    /// Gets the core ordering direction based on the defined direction.
    /// </summary>
    /// <returns>The core ordering direction.</returns>
    public Core.OrderingDirection GetOrderingDirection()
    {
        switch (Direction)
        {
            case OrderDirection.Ascending:
                return Core.OrderingDirection.Ascending;
            case OrderDirection.Descending:
                return Core.OrderingDirection.Descending;
            default:
                throw new Exception("Invalid ordering direction.");
        }
    }
}

public static class OrderGenerator
{
    //*
    // { order: { prop: "ascending", prop2: { subProp: "descending" } } }
    //*
    public static IEnumerable<OrderDefinition> Translate(GeneratorContext context, Node node)
    {
        if (node is not ScopeDefinitionNode scope)
        {
            throw new GeneratorException("The 'order' node must be a scope definition (object) with ordering expressions.", context);
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

    private static IEnumerable<OrderDefinition> TranslateScopeExpression(GeneratorContext context, ScopeDefinitionNode node)
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

    private static OrderDefinition TranslateLiteralExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not LiteralNode literal)
        {
            throw new GeneratorException($"Ordering Error: The right-hand side of the order expression for '{node.Lhs.Value}' must be a literal indicating the sort direction ('ascending' or 'descending').", context);
        }

        var memberName = node.Lhs.Value;
        var subContext = context.CreateSubContext(memberName);

        return new OrderDefinition(subContext.Type, subContext.Expression, GetOrderDirection(subContext, literal));
    }

    private static string? TranslateStringLiteral(LiteralNode node)
    {
        return node.Value?[1..^1];
    }

    private static OrderDirection GetOrderDirection(GeneratorContext context, LiteralNode node)
    {
        if (TranslateStringLiteral(node) == "ascending")
        {
            return OrderDirection.Ascending;
        }
        if (TranslateStringLiteral(node) == "descending")
        {
            return OrderDirection.Descending;
        }

        throw new GeneratorException($"Invalid order direction: '{node.Value}'. Expected 'ascending' or 'descending'.", context);
    }
}
