using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class ArithmeticOperationExpressionTranslator
{
    public static Expression TranslateArithmeticOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Add:
                return TranslateAddExpression(context, node);

            case WebqlOperatorType.Subtract:
                return TranslateSubtractExpression(context, node);

            case WebqlOperatorType.Divide:
                return TranslateDivideExpression(context, node);

            case WebqlOperatorType.Multiply:
                return TranslateMultiplyExpression(context, node);

            case WebqlOperatorType.Modulo:
                return TranslateModuloExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateAddExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateSubtractExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateDivideExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateMultiplyExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateModuloExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



