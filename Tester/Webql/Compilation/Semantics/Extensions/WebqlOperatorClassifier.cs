using Webql.Parsing.Components;

namespace Webql.Semantics.Tools;

public static class WebqlOperatorClassifier
{
    public static WebqlOperatorCategory GetOperatorCategory(WebqlOperatorType operatorType)
    {
        switch (operatorType)
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

    public static bool IsArithmetic(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.Arithmetic;
    }

    public static bool IsRelational(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.Relational;
    }

    public static bool IsStringRelational(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.StringRelational;
    }

    public static bool IsLogical(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.Logical;
    }

    public static bool IsCollectionManipulation(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.CollectionManipulation;
    }

    public static bool IsCollectionAggregation(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.CollectionAggregation;
    }

    public static WebqlOperatorArity GetOperatorArity(WebqlOperatorType operatorType)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Not:
            case WebqlOperatorType.Min:
            case WebqlOperatorType.Max:
            case WebqlOperatorType.Sum:
            case WebqlOperatorType.Average:
                return WebqlOperatorArity.Unary;

            case WebqlOperatorType.Add:
            case WebqlOperatorType.Subtract:
            case WebqlOperatorType.Divide:
            case WebqlOperatorType.Multiply:
            case WebqlOperatorType.Modulo:
            case WebqlOperatorType.Equals:
            case WebqlOperatorType.NotEquals:
            case WebqlOperatorType.Less:
            case WebqlOperatorType.LessEquals:
            case WebqlOperatorType.Greater:
            case WebqlOperatorType.GreaterEquals:
            case WebqlOperatorType.Like:
            case WebqlOperatorType.RegexMatch:
            case WebqlOperatorType.Or:
            case WebqlOperatorType.And:
            case WebqlOperatorType.Filter:
            case WebqlOperatorType.Select:
            case WebqlOperatorType.SelectMany:
            case WebqlOperatorType.Limit:
            case WebqlOperatorType.Skip:
            case WebqlOperatorType.Count:
            case WebqlOperatorType.Index:
            case WebqlOperatorType.Any:
            case WebqlOperatorType.All:
                return WebqlOperatorArity.Binary;

            default:
                throw new InvalidOperationException();
        }
    }

    public static bool IsNullary(WebqlOperatorType operatorType)
    {
        return GetOperatorArity(operatorType) == WebqlOperatorArity.Nullary;
    }

    public static bool IsUnary(WebqlOperatorType operatorType)
    {
        return GetOperatorArity(operatorType) == WebqlOperatorArity.Unary;
    }

    public static bool IsBinary(WebqlOperatorType operatorType)
    {
        return GetOperatorArity(operatorType) == WebqlOperatorArity.Binary;
    }

    public static bool IsTernary(WebqlOperatorType operatorType)
    {
        return GetOperatorArity(operatorType) == WebqlOperatorArity.Ternary;
    }

    /*
     * Type Based Classification
     */

    public static WebqlCollectionManipulationOperator GetCollectionManipulationOperator(WebqlOperatorType operatorType)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Filter:
                return WebqlCollectionManipulationOperator.Filter;

            case WebqlOperatorType.Select:
                return WebqlCollectionManipulationOperator.Select;

            case WebqlOperatorType.SelectMany:
                return WebqlCollectionManipulationOperator.SelectMany;

            case WebqlOperatorType.Limit:
                return WebqlCollectionManipulationOperator.Limit;

            case WebqlOperatorType.Skip:
                return WebqlCollectionManipulationOperator.Skip;

            default:
                throw new InvalidOperationException();
        }
    }

    public static WebqlCollectionAggregationOperator GetCollectionAggregationOperator(WebqlOperatorType operatorType)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Count:
                return WebqlCollectionAggregationOperator.Count;

            case WebqlOperatorType.Index:
                return WebqlCollectionAggregationOperator.Index;

            case WebqlOperatorType.Any:
                return WebqlCollectionAggregationOperator.Any;

            case WebqlOperatorType.All:
                return WebqlCollectionAggregationOperator.All;

            case WebqlOperatorType.Min:
                return WebqlCollectionAggregationOperator.Min;

            case WebqlOperatorType.Max:
                return WebqlCollectionAggregationOperator.Max;

            case WebqlOperatorType.Sum:
                return WebqlCollectionAggregationOperator.Sum;

            case WebqlOperatorType.Average:
                return WebqlCollectionAggregationOperator.Average;

            default:
                throw new InvalidOperationException();
        }
    }

}
