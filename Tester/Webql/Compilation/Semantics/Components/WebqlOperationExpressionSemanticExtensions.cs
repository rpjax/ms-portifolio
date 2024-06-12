using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Semantics.Components;

public static class WebqlOperationExpressionSemanticExtensions
{
    public static WebqlOperatorCategory GetOperatorCategory(this WebqlOperationExpression expression)
    {
        switch (expression.Operator)
        {
            case WebqlOperatorType.Add:
            case WebqlOperatorType.Subtract:
            case WebqlOperatorType.Divide:
            case WebqlOperatorType.Multiply:
            case WebqlOperatorType.Modulo:
                return WebqlOperatorCategory.Arithmetic;

            case WebqlOperatorType.Equals:
            case WebqlOperatorType.NotEquals:
            case WebqlOperatorType.Less:
            case WebqlOperatorType.LessEquals:
            case WebqlOperatorType.Greater:
            case WebqlOperatorType.GreaterEquals:
                return WebqlOperatorCategory.Relational;

            case WebqlOperatorType.Like:
            case WebqlOperatorType.RegexMatch:
                return WebqlOperatorCategory.StringRelational;

            case WebqlOperatorType.Or:
            case WebqlOperatorType.And:
            case WebqlOperatorType.Not:
                return WebqlOperatorCategory.Logical;

            case WebqlOperatorType.Filter:
            case WebqlOperatorType.Select:
            case WebqlOperatorType.SelectMany:
            case WebqlOperatorType.Limit:
            case WebqlOperatorType.Skip:
                return WebqlOperatorCategory.CollectionManipulation;

            case WebqlOperatorType.Count:
            case WebqlOperatorType.Index:
            case WebqlOperatorType.Any:
            case WebqlOperatorType.All:
            case WebqlOperatorType.Min:
            case WebqlOperatorType.Max:
            case WebqlOperatorType.Sum:
            case WebqlOperatorType.Average:
                return WebqlOperatorCategory.CollectionAggregation;

            default:
                throw new InvalidOperationException();
        }
    }
}