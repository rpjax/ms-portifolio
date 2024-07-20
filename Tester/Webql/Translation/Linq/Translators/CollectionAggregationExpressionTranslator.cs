using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class CollectionAggregationExpressionTranslator
{
    public static Expression TranslateCollectionAggregationExpression(WebqlOperationExpression node)
    {
        switch (node.GetCollectionAggregationOperator())
        {
            case WebqlCollectionAggregationOperator.Count:
                return TranslateCountExpression(node);

            case WebqlCollectionAggregationOperator.Contains:
                return TranslateContainsExpression(node);

            case WebqlCollectionAggregationOperator.Index:
                return TranslateIndexExpression(node);

            case WebqlCollectionAggregationOperator.Any:
                return TranslateAnyExpression(node);

            case WebqlCollectionAggregationOperator.All:
                return TranslateAllExpression(node);

            case WebqlCollectionAggregationOperator.Min:
                return TranslateMinExpression(node);

            case WebqlCollectionAggregationOperator.Max:
                return TranslateMaxExpression(node);

            case WebqlCollectionAggregationOperator.Sum:
                return TranslateSumExpression(node);

            case WebqlCollectionAggregationOperator.Average:
                return TranslateAverageExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    public static Expression TranslateCountExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetCountMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateContainsExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetContainsMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateIndexExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetIndexMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateAnyExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAnyMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateAllExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAllMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateMinExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetMinMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateMaxExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetMaxMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateSumExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetSumMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    public static Expression TranslateAverageExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAverageMethodInfo(lhsSemantics.Type);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

}
