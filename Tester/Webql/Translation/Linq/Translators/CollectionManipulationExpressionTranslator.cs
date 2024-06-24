using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class CollectionManipulationExpressionTranslator
{
    public static Expression TranslateCollectionManipulationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Filter:
                return TranslateFilterExpression(context, node);

            case WebqlOperatorType.Select:
                return TranslateSelectExpression(context, node);

            case WebqlOperatorType.SelectMany:
                return TranslateSelectManyExpression(context, node);

            case WebqlOperatorType.Limit:
                return TranslateLimitExpression(context, node);

            case WebqlOperatorType.Skip:
                return TranslateSkipExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    public static Expression TranslateFilterExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateSelectExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateSelectManyExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateLimitExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateSkipExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}
