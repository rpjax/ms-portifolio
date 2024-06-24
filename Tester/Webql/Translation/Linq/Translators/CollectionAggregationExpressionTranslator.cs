using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq;

public static class CollectionAggregationExpressionTranslator
{
    public static Expression TranslateCollectionAggregationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Count:
                return TranslateCountExpression(context, node);

            case WebqlOperatorType.Index:
                return TranslateIndexExpression(context, node);

            case WebqlOperatorType.Any:
                return TranslateAnyExpression(context, node);

            case WebqlOperatorType.All:
                return TranslateAllExpression(context, node);

            case WebqlOperatorType.Min:
                return TranslateMinExpression(context, node);

            case WebqlOperatorType.Max:
                return TranslateMaxExpression(context, node);

            case WebqlOperatorType.Sum:
                return TranslateSumExpression(context, node);

            case WebqlOperatorType.Average:
                return TranslateAverageExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    public static Expression TranslateCountExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateIndexExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateAnyExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateAllExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateMinExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateMaxExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateSumExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateAverageExpression(TranslationContext context, WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

}
