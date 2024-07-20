using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class CollectionManipulationExpressionTranslator
{
    public static Expression TranslateCollectionManipulationExpression(WebqlOperationExpression node)
    {
        switch (node.GetCollectionManipulationOperator())
        {
            case WebqlCollectionManipulationOperator.Filter:
                return TranslateFilterExpression(node);

            case WebqlCollectionManipulationOperator.Select:
                return TranslateSelectExpression(node);

            case WebqlCollectionManipulationOperator.SelectMany:
                return TranslateSelectManyExpression(node);

            case WebqlCollectionManipulationOperator.Limit:
                return TranslateLimitExpression(node);

            case WebqlCollectionManipulationOperator.Skip:
                return TranslateSkipExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    public static Expression TranslateFilterExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var elementParameter = rhs.GetElementParameterExpression();
        var lambdaExpression = Expression.Lambda(rhsExpression, elementParameter);

        var methodInfo = node.GetLinqProvider()
            .GetWhereMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, lambdaExpression);
    }

    public static Expression TranslateSelectExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var elementParameter = rhs.GetElementParameterExpression();
        var lambdaExpression = Expression.Lambda(rhsExpression, elementParameter);

        var methodInfo = node.GetLinqProvider()
            .GetSelectMethodInfo(source: lhs, selector: rhs);

        return Expression.Call(methodInfo, lhsExpression, lambdaExpression);
    }

    public static Expression TranslateSelectManyExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateLimitExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = node.GetLinqProvider()
            .GetTakeMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateSkipExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = node.GetLinqProvider()
            .GetSkipMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }
}
