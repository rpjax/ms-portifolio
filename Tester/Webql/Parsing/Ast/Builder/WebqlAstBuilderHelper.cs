using ModularSystem.Core.TextAnalysis.Parsing.Components;
using System.Runtime.CompilerServices;

namespace Webql.Parsing.Ast.Builder;

public static class WebqlAstBuilderHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CstExpressionType GetCstExpressionType(CstInternal node)
    {
        switch (node.Name)
        {
            case "literal_expression":
                return CstExpressionType.Literal;

            case "reference_expression":
                return CstExpressionType.Reference;

            case "scope_access_expression":
                return CstExpressionType.ScopeAccess;

            case "block_expression":
                return CstExpressionType.Block;

            case "operation_expression":
                return CstExpressionType.Operation;

            case "anonymous_object_expression":
                return CstExpressionType.AnonymousObject;

            default:
                throw new InvalidOperationException($"Invalid expression type: {node.Name}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
             * semantic operators
             */
            case "aggregate":
                return WebqlOperatorType.Aggregate;

            case "new":
                return WebqlOperatorType.New;

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

            case "contains":
                return WebqlOperatorType.Contains;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlScopeType GetOperatorScopeType(WebqlOperatorType operatorType, WebqlScopeType defaultValue)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Filter:
                return WebqlScopeType.LogicalFiltering;

            case WebqlOperatorType.Select:
            case WebqlOperatorType.SelectMany:
                return WebqlScopeType.Projection;

            case WebqlOperatorType.Aggregate:
                return WebqlScopeType.Aggregation;

            default:
                return defaultValue;
        }
    }

}

public enum CstOperatorArity
{
    Unary,
    Binary,
    Special
}