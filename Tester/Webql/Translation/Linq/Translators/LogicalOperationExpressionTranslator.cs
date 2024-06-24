using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class LogicalOperationExpressionTranslator
{
    public static Expression TranslateLogicalOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Or:
                return TranslateOrExpression(context, node);

            case WebqlOperatorType.And:
                return TranslateAndExpression(context, node);

            case WebqlOperatorType.Not:
                return TranslateNotExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateOrExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateAndExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateNotExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



