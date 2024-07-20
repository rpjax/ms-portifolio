using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class RelationalOperationExpressionTranslator
{
    public static Expression TranslateRelationalOperationExpression(WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Equals:
                return TranslateEqualsExpression(node);

            case WebqlOperatorType.NotEquals:
                return TranslateNotEqualsExpression(node);

            case WebqlOperatorType.Less:
                return TranslateLessExpression(node);

            case WebqlOperatorType.LessEquals:
                return TranslateLessEqualsExpression(node);

            case WebqlOperatorType.Greater:
                return TranslateGreaterExpression(node);

            case WebqlOperatorType.GreaterEquals:
                return TranslateGreaterEqualsExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateEqualsExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.Equal(lhsExpression, rhsExpression);
    }

    public static Expression TranslateNotEqualsExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.NotEqual(lhsExpression, rhsExpression);
    }

    public static Expression TranslateLessExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.LessThan(lhsExpression, rhsExpression);
    }

    public static Expression TranslateLessEqualsExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.LessThanOrEqual(lhsExpression, rhsExpression);
    }

    public static Expression TranslateGreaterExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.GreaterThan(lhsExpression, rhsExpression);
    }

    public static Expression TranslateGreaterEqualsExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.GreaterThanOrEqual(lhsExpression, rhsExpression);
    }
}
