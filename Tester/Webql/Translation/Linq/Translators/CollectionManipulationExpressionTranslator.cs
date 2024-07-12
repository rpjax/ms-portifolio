using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Definitions;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class CollectionManipulationExpressionTranslator
{
    public static Expression TranslateCollectionManipulationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Filter:
                return TranslateFilterExpression(node);

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

    public static Expression TranslateFilterExpression(WebqlOperationExpression node)
    {
        var compilationContext = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var methodInfo = compilationContext.MethodInfoProvider.GetWhereMethodInfo(lhsSemantics.Type);

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var elementParameter = rhs.GetElementParameterExpression();
        var lambdaExpression = Expression.Lambda(rhsExpression, elementParameter);

        var whereExpression = Expression.Call(methodInfo, lhsExpression, lambdaExpression);

        return whereExpression;
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
