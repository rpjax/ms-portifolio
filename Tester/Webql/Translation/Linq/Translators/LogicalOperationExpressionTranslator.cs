using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class LogicalOperationExpressionTranslator
{
    public static Expression TranslateLogicalOperationExpression(WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Or:
                return TranslateOrExpression(node);

            case WebqlOperatorType.And:
                return TranslateAndExpression(node);

            case WebqlOperatorType.Not:
                return TranslateNotExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateOrExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.OrElse(lhsExpression, rhsExpression);
    }

    public static Expression TranslateAndExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        return Expression.AndAlso(lhsExpression, rhsExpression);
    }

    public static Expression TranslateNotExpression(WebqlOperationExpression node)
    {
        var operand = node.Operands[0];
        var expression = ExpressionTranslator.TranslateExpression(operand);

        return Expression.Not(expression);
    }
}

