using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast;

public static class WebqlAstBuilderHelper
{
    public static WebqlExpressionType GetCstExpressionType(CstInternal node)
    {
        switch (node.Name)
        {
            case "literal_expression":
                return WebqlExpressionType.Literal;

            case "reference_expression":
                return WebqlExpressionType.Reference;

            case "scope_access_expression":
                return WebqlExpressionType.ScopeAccess;

            case "block_expression":
                return WebqlExpressionType.Block;

            case "operation_expression":
                return WebqlExpressionType.Operation;

            default:
                throw new InvalidOperationException();
        }
    }

    public static WebqlOperatorType GetCstOperatorType(string name)
    {
        switch (name)
        {
            /*
             * arithmetic operators
             */
            case "add":
                return WebqlOperatorType.Add;

            case "subtract":
                return WebqlOperatorType.Subtract;

            case "multiply":
                return WebqlOperatorType.Multiply;

            case "divide":
                return WebqlOperatorType.Divide;

            /*
             * relational operators
             */
            case "equals":
                return WebqlOperatorType.Equals;

            case "notEquals":
                return WebqlOperatorType.NotEquals;

            case "less":
                return WebqlOperatorType.Less;

            case "lessEquals":
                return WebqlOperatorType.LessEquals;

            case "greater":
                return WebqlOperatorType.Greater;

            case "greaterEquals":
                return WebqlOperatorType.GreaterEquals;

            /*
             * string relational operators
             */
            case "like":
                return WebqlOperatorType.Like;

            case "regexMatch":
                return WebqlOperatorType.RegexMatch;

            /*
             * logical operators
             */
            case "or":
                return WebqlOperatorType.Or;

            case "and":
                return WebqlOperatorType.And;

            case "not":
                return WebqlOperatorType.Not;

            /*
             * collection manipulation operators
             */
            case "filter":
                return WebqlOperatorType.Filter;

            case "select":
                return WebqlOperatorType.Select;

            case "selectMany":
                return WebqlOperatorType.SelectMany;

            case "limit":
                return WebqlOperatorType.Limit;

            case "skip":
                return WebqlOperatorType.Skip;

            /*
             * collection aggregation operators
             */
            case "count":
                return WebqlOperatorType.Count;

            case "index":
                return WebqlOperatorType.Index;

            case "any":
                return WebqlOperatorType.Any;

            case "all":
                return WebqlOperatorType.All;

            case "min":
                return WebqlOperatorType.Min;

            case "max":
                return WebqlOperatorType.Max;

            case "sum":
                return WebqlOperatorType.Sum;

            case "average":
                return WebqlOperatorType.Average;

            default:
                throw new InvalidOperationException("Invalid operator type");
        }
    }

}
