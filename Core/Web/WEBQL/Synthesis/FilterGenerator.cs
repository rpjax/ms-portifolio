using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

//*
// Generator for helper components.
//*
public static partial class FilterGenerator
{
    public static Operator GetOperatorFromLhs(LhsNode node)
    {
        var operators = Enum.GetValues(typeof(Operator));

        foreach (Operator op in operators)
        {
            if (HelperTools.Stringify(op) == node.Value.ToCamelCase())
            {
                return op.TypeCast<Operator>();
            }
        }

        throw new GeneratorException($"The operator '{node.Value}' is not recognized or supported. Please ensure it is a valid operator.", node);
    }
}

//*
// Generator for the nodes.
//*
public static partial class FilterGenerator
{
    public static Expression Translate(GeneratorContext context, Node node)
    {
        switch (node.NodeType)
        {
            case NodeType.Literal:
                return TranslateLiteral(context, node.As<LiteralNode>());

            case NodeType.Array:
                return TranslateScopeArray(context, node.As<ArrayNode>(), BinaryOperator.And);

            case NodeType.LeftHandSide:
                throw new GeneratorException("A left-hand side (LHS) node cannot be used as the root node in the translation process. Node: " + node.ToString(), node);

            case NodeType.RightHandSide:
                throw new GeneratorException("A right-hand side (RHS) node cannot be used as the root node in the translation process. Node: " + node.ToString(), node);

            case NodeType.Expression:
                return TranslateExpression(context, node.As<ExpressionNode>());

            case NodeType.ScopeDefinition:
                return TranslateScopeDefinition(context, node.As<ScopeDefinitionNode>());

            default:
                throw new GeneratorException("The node type '" + node.NodeType + "' is not supported as the root in the translation context. Node: " + node.NodeType.ToString(), node);
        }
    }

    public static Expression TranslateScopeDefinition(GeneratorContext context, ScopeDefinitionNode node)
    {
        var childrenNodes = node.Expressions;
        var expression = null as Expression;

        foreach (var item in childrenNodes)
        {
            var subExpression = TranslateExpression(context, item);

            if (subExpression == null)
            {
                continue;
            }

            if (expression == null)
            {
                expression = subExpression;
            }
            else
            {
                expression = Expression.AndAlso(expression, subExpression);
            }
        }

        if (expression == null)
        {
            return Expression.Constant(true);
        }

        return expression;
    }

    private static Expression TranslateScopeArray(GeneratorContext context, ArrayNode array, BinaryOperator op)
    {
        foreach (var item in array.Values)
        {
            if (item is not ScopeDefinitionNode)
            {
                throw new GeneratorException("Each item in the array must be a scope definition. Invalid item found.", item);
            }
        }

        var translations = array.Values
            .Transform(x => x.TypeCast<ScopeDefinitionNode>())
            .ToArray();

        if (translations.Length == 0)
        {
            return Expression.Constant(true);
        }
        if (translations.Length == 1)
        {
            return TranslateScopeDefinition(context, translations.First());
        }

        var expression = null as Expression;

        foreach (var item in translations)
        {
            var translation = TranslateScopeDefinition(context, item);

            if (expression == null)
            {
                expression = translation;
            }
            else
            {
                if (op == BinaryOperator.Or)
                {
                    expression = Expression.OrElse(expression, translation);
                }
                else if (op == BinaryOperator.And)
                {
                    expression = Expression.AndAlso(expression, translation);
                }
                else
                {
                    throw new GeneratorException("Unsupported binary operator '" + op + "' in scope array translation.", item);
                }
            }
        }

        if (expression == null)
        {
            throw new InvalidOperationException("No valid expression could be generated from the scope array.");
        }

        return expression;
    }

    private static Expression TranslateExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Lhs.IsOperator)
        {
            return TranslateOperatorExpression(context, node);
        }

        return TranslateMemberExpression(context, node);
    }

    private static Expression TranslateMemberExpression(GeneratorContext context, ExpressionNode node)
    {
        var memberName = node.Lhs.Value;
        var subContext = context.CreateSubContext(memberName);

        if (node.Rhs.Value is not ScopeDefinitionNode scope)
        {
            throw new GeneratorException("The right-hand side of the member expression '" + memberName + "' must be a scope definition (object).", node);
        }

        return TranslateScopeDefinition(subContext, scope);
    }

    private static Expression TranslateOperatorExpression(GeneratorContext context, ExpressionNode node)
    {
        switch (GetOperatorFromLhs(node.Lhs))
        {
            case Operator.Equals:
                return TranslateEqualsExpression(context, node);

            case Operator.Not:
                return TranslateNotEqualsExpression(context, node);

            case Operator.Less:
                return TranslateLesserExpression(context, node);

            case Operator.LesserEquals:
                return TranslateLesserEqualsExpression(context, node);

            case Operator.Greater:
                return TranslateGreaterExpression(context, node);

            case Operator.GreaterEquals:
                return TranslateGreaterEqualsExpression(context, node);

            case Operator.Like:
                return TranslateLikeExpression(context, node);

            case Operator.Any:
                return TranslateAnyExpression(context, node);

            case Operator.All:
                return TranslateAllExpression(context, node);

            default:
                throw new GeneratorException("Unknown or unsupported operator in the expression.", node.Lhs);

        }
    }

    public static Expression TranslateLiteral(GeneratorContext context, LiteralNode node)
    {
        var type = context.Type;

        if (node.Value == null)
        {
            return Expression.Constant(null, type);
        }

        var value = JsonSerializerSingleton.Deserialize(node.Value, type);

        return Expression.Constant(value, type);
    }

    public static string? TranslateStringLiteral(LiteralNode node)
    {
        return node.Value?[1..^1];
    }
}

//*
// Generator for the operatos.
//*
public static partial class FilterGenerator
{
    private static Expression TranslateEqualsExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of an '{HelperTools.Stringify(Operator.Equals)}' expression must be a literal value.", rhs);
        }

        return Expression.Equal(context.Expression, TranslateLiteral(context, literal));
    }

    private static Expression TranslateNotEqualsExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.Not)}' expression must be a literal value.", rhs);
        }

        return Expression.NotEqual(context.Expression, TranslateLiteral(context, literal));
    }

    private static Expression TranslateLesserExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.Less)}' expression must be a literal value.", rhs);
        }

        return Expression.LessThan(context.Expression, TranslateLiteral(context, literal));
    }

    private static Expression TranslateLesserEqualsExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.LesserEquals)}' expression must be a literal value.", rhs);
        }

        return Expression.LessThanOrEqual(context.Expression, TranslateLiteral(context, literal));
    }

    private static Expression TranslateGreaterExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.Greater)}' expression must be a literal value.", rhs);
        }

        return Expression.GreaterThan(context.Expression, TranslateLiteral(context, literal));
    }

    private static Expression TranslateGreaterEqualsExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.GreaterEquals)}' expression must be a literal value.", rhs);
        }

        var literalExpression = TranslateLiteral(context, literal);

        return Expression.GreaterThanOrEqual(context.Expression, literalExpression);
    }

    //*
    // Translates the "$like" operator expression
    //*
    private static Expression TranslateLikeExpression(GeneratorContext context, ExpressionNode node)
    {
        var rhs = node.Rhs.Value;

        if (rhs is not LiteralNode literal)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.Like)}' expression must be a literal value.", rhs);
        }

        var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });
        var toLowerArgs = new ConstantExpression[] { };
        var tolowerExpression = Expression.Call(context.Expression, toLowerMethod!, toLowerArgs);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        if (literal.Value == null)
        {
            return Expression.Equal(context.Expression, TranslateLiteral(context, literal));
        }

        var str = TranslateStringLiteral(literal);

        if (str == null)
        {
            throw new GeneratorException($"The string value in the '{HelperTools.Stringify(Operator.Like)}' expression is invalid or cannot be processed.", rhs);
        }

        var containsArgs = new[] { Expression.Constant(str.ToLower(), typeof(string)) };

        return Expression.Call(tolowerExpression, containsMethod!, containsArgs);
    }

    //*
    // Translates the "$any" operator expression
    //*
    private static Expression TranslateAnyExpression(GeneratorContext context, ExpressionNode node)
    {
        if (HelperTools.TypeIsEnumerable(context.Type))
        {
            return TranslateEnumerableAnyExpression(context, node);
        }
        else
        {
            return TranslateNonEnumerableAnyExpression(context, node);
        }
    }

    private static Expression TranslateEnumerableAnyExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new GeneratorException($"The right-hand side of a {HelperTools.Stringify(Operator.Any)} expression must be an array of conditions.", node.Rhs.Value);
        }

        var enumType = HelperTools.GetEnumerableType(context.Type);
        var lambdaParam = Expression.Parameter(enumType, "y");
        var subContext = new GeneratorContext(enumType, lambdaParam, context);
        var body = TranslateScopeArray(subContext, array, BinaryOperator.Or);
        var lambda = Expression.Lambda(body, lambdaParam);
        var args = new Expression[]
        {
            context.Expression,
            lambda
        };
        var typeArgs = new Type[] { enumType };

        return Expression.Call(typeof(Enumerable), "Any", typeArgs, args);
    }

    private static Expression TranslateNonEnumerableAnyExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new GeneratorException("The right-hand side of a non-enumerable 'Any' expression must be an array of conditions.", node.Rhs.Value);
        }

        return TranslateScopeArray(context, array, BinaryOperator.Or);
    }

    //*
    // Translates the "$all" operator expression
    //*
    private static Expression TranslateAllExpression(GeneratorContext context, ExpressionNode node)
    {
        if (HelperTools.TypeIsEnumerable(context.Type))
        {
            return TranslateEnumerableAllExpression(context, node);
        }
        else
        {
            return TranslateNonEnumerableAllExpression(context, node);
        }
    }

    private static Expression TranslateEnumerableAllExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.All)}' expression must be an array of conditions for an enumerable type.", node.Rhs.Value);
        }

        var enumType = HelperTools.GetEnumerableType(context.Type);
        var lambdaParam = Expression.Parameter(enumType, "y");
        var subContext = new GeneratorContext(enumType, lambdaParam, context);
        var body = TranslateScopeArray(subContext, array, BinaryOperator.And);
        var lambda = Expression.Lambda(body, lambdaParam);
        var args = new Expression[]
        {
            context.Expression,
            lambda
        };
        var typeArgs = new Type[] { enumType };

        return Expression.Call(typeof(Enumerable), "All", typeArgs, args);
    }

    private static Expression TranslateNonEnumerableAllExpression(GeneratorContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new GeneratorException($"The right-hand side of a '{HelperTools.Stringify(Operator.All)}' expression must be an array of conditions for a non-enumerable type.", node.Rhs.Value);
        }

        return TranslateScopeArray(context, array, BinaryOperator.And);
    }

}