using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;
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
        var translationContext = node.GetTranslationContext();

        var leftExpression = translationContext.GetLeftHandSideExpression();
        var righExpression = ExpressionTranslator.TranslateExpression(node.Operands[0]);

        return Expression.Add(leftExpression, righExpression);
    }

    public static Expression TranslateSubtractExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateDivideExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateMultiplyExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateModuloExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



