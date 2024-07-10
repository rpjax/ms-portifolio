using Webql.Parsing.Ast;

namespace Webql.Core.Analysis;

public static class WebqlOperatorAnalyzer
{
    /*
     * Operator Classification
     */

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

            case WebqlOperatorType.Aggregate:
                return WebqlOperatorCategory.Semantic;

            case WebqlOperatorType.Filter:
            case WebqlOperatorType.Select:
            case WebqlOperatorType.SelectMany:
            case WebqlOperatorType.Limit:
            case WebqlOperatorType.Skip:
                return WebqlOperatorCategory.CollectionManipulation;

            case WebqlOperatorType.Count:
            case WebqlOperatorType.Contains:
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

    public static WebqlOperatorArity GetOperatorArity(WebqlOperatorType operatorType)
    {
        /*
         * Currently, all operators are binary.
         */

        return WebqlOperatorArity.Binary;

        switch (operatorType)
        {
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
            case WebqlOperatorType.Contains:
            case WebqlOperatorType.Index:
            case WebqlOperatorType.Any:
            case WebqlOperatorType.All:
            case WebqlOperatorType.Not:
            case WebqlOperatorType.Aggregate:
            case WebqlOperatorType.Min:
            case WebqlOperatorType.Max:
            case WebqlOperatorType.Sum:
            case WebqlOperatorType.Average:
                return WebqlOperatorArity.Binary;

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * Operator Subtype Classification
     */

    public static WebqlSemanticOperator GetSemanticOperator(WebqlOperatorType operatorType)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Aggregate:
                return WebqlSemanticOperator.Aggregate;

            default:
                throw new InvalidOperationException();
        }
    }

    public static WebqlCollectionOperator GetCollectionOperator(WebqlOperatorType operatorType)
    {
        switch (operatorType)
        {
            case WebqlOperatorType.Filter:
                return WebqlCollectionOperator.Filter;
            case WebqlOperatorType.Select:
                return WebqlCollectionOperator.Select;
            case WebqlOperatorType.SelectMany:
                return WebqlCollectionOperator.SelectMany;
            case WebqlOperatorType.Limit:
                return WebqlCollectionOperator.Limit;
            case WebqlOperatorType.Skip:
                return WebqlCollectionOperator.Skip;
            case WebqlOperatorType.Count:
                return WebqlCollectionOperator.Count;
            case WebqlOperatorType.Contains:
                return WebqlCollectionOperator.Contains;
            case WebqlOperatorType.Index:
                return WebqlCollectionOperator.Index;
            case WebqlOperatorType.Any:
                return WebqlCollectionOperator.Any;
            case WebqlOperatorType.All:
                return WebqlCollectionOperator.All;
            case WebqlOperatorType.Min:
                return WebqlCollectionOperator.Min;
            case WebqlOperatorType.Max:
                return WebqlCollectionOperator.Max;
            case WebqlOperatorType.Sum:
                return WebqlCollectionOperator.Sum;
            case WebqlOperatorType.Average:
                return WebqlCollectionOperator.Average;

            default:
                throw new InvalidOperationException();
        }
    }

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

    /*
     * Helper Methods
     */

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

    public static bool IsCollectionManipulationOperator(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.CollectionManipulation;
    }

    public static bool IsCollectionAggregationOperator(WebqlOperatorType operatorType)
    {
        return GetOperatorCategory(operatorType) == WebqlOperatorCategory.CollectionAggregation;
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

    public static bool IsCollectionOperator(this WebqlOperatorType @operator)
    {
        var operatorCategory = GetOperatorCategory(@operator);

        return false
            || operatorCategory == WebqlOperatorCategory.CollectionManipulation
            || operatorCategory == WebqlOperatorCategory.CollectionAggregation;
    }

}
