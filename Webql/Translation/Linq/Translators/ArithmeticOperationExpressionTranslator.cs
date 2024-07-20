using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class ArithmeticOperationExpressionTranslator
{
    public static Expression TranslateArithmeticOperationExpression(WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Add:
                return TranslateAddExpression(node);

            case WebqlOperatorType.Subtract:
                return TranslateSubtractExpression(node);

            case WebqlOperatorType.Divide:
                return TranslateDivideExpression(node);

            case WebqlOperatorType.Multiply:
                return TranslateMultiplyExpression(node);

            case WebqlOperatorType.Modulo:
                return TranslateModuloExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateAddExpression(WebqlOperationExpression node)
    {
        var leftExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[1]);

        return Expression.Add(leftExpression, righExpression);
    }

    public static Expression TranslateSubtractExpression(WebqlOperationExpression node)
    {
        var leftExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[1]);

        return Expression.Subtract(leftExpression, righExpression);
    }

    public static Expression TranslateDivideExpression(WebqlOperationExpression node)
    {
        var leftExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[1]);

        return Expression.Divide(leftExpression, righExpression);
    }

    public static Expression TranslateMultiplyExpression(WebqlOperationExpression node)
    {
        var leftExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[1]);

        return Expression.Multiply(leftExpression, righExpression);
    }

    public static Expression TranslateModuloExpression(WebqlOperationExpression node)
    {
        var leftExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[1]);

        return Expression.Modulo(leftExpression, righExpression);
    }
}



