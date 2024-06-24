using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class RelationalOperationExpressionTranslator
{
    public static Expression TranslateRelationalOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Equals:
                return TranslateEqualsExpression(context, node);

            case WebqlOperatorType.NotEquals:
                return TranslateNotEqualsExpression(context, node);

            case WebqlOperatorType.Less:
                return TranslateLessExpression(context, node);

            case WebqlOperatorType.LessEquals:
                return TranslateLessEqualsExpression(context, node);

            case WebqlOperatorType.Greater:
                return TranslateGreaterExpression(context, node);

            case WebqlOperatorType.GreaterEquals:
                return TranslateGreaterEqualsExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateEqualsExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateNotEqualsExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateLessExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateLessEqualsExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateGreaterExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateGreaterEqualsExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



