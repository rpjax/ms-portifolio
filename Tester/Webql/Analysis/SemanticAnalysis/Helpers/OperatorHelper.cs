using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Helpers;

public static class OperatorHelper
{
    //*
    //* GOOD STUFF.
    //*

    public static OperatorSemanticType GetOperatorSemanticType(OperatorType @operator)
    {
        switch (@operator)
        {
            // Arithmetic operators
            case OperatorType.Add:
            case OperatorType.Subtract:
            case OperatorType.Divide:
            case OperatorType.Multiply:
            case OperatorType.Modulo:
                return OperatorSemanticType.Arithmetic;

            // Relational Operators
            case OperatorType.Equals:
            case OperatorType.NotEquals:
            case OperatorType.Less:
            case OperatorType.LessEquals:
            case OperatorType.Greater:
            case OperatorType.GreaterEquals:
                return OperatorSemanticType.Relational;

            // String Relational Operators
            case OperatorType.Like:
            case OperatorType.RegexMatch:
                return OperatorSemanticType.StringRelational;

            // Logical Operators
            case OperatorType.Or:
            case OperatorType.And:
            case OperatorType.Not:
                return OperatorSemanticType.Logical;

            // Semantic Operators
            //case OperatorType.Expr:
            //case OperatorType.Parse:
            case OperatorType.Type:
            case OperatorType.MemberAccess:
                return OperatorSemanticType.Semantic;

            // Collection Manipulation Operators
            case OperatorType.Filter:
            case OperatorType.Select:
            //case OperatorType.Transform:
            case OperatorType.SelectMany:
            case OperatorType.Limit:
            case OperatorType.Skip:
                return OperatorSemanticType.CollectionManipulation;

            // Collection Aggregation Operators
            case OperatorType.Count:
            case OperatorType.Index:
            case OperatorType.Any:
            case OperatorType.All:
            case OperatorType.Min:
            case OperatorType.Max:
            case OperatorType.Sum:
            case OperatorType.Average:
                return OperatorSemanticType.CollectionAggregation;
        }

        throw new InvalidOperationException();
    }

    public static LogicalOperatorType GetLogicalOperatorType(OperatorType @operator)
    {
        switch (@operator)
        {
            case OperatorType.Or:
                return LogicalOperatorType.Or;

            case OperatorType.And:
                return LogicalOperatorType.And;

            case OperatorType.Not:
                return LogicalOperatorType.Not;
        }

        throw new InvalidOperationException();
    }

    public static SemanticOperatorType GetSemanticOperatorType(OperatorType @operator)
    {
        switch (@operator)
        {
            //case OperatorType.Expr:
            //    return SemanticOperatorType.Expr;

            //case OperatorType.Parse:
            //    return SemanticOperatorType.Parse;

            case OperatorType.Type:
                return SemanticOperatorType.Type;

            case OperatorType.MemberAccess:
                return SemanticOperatorType.MemberAccess;
        }

        throw new InvalidOperationException();
    }

    public static CollectionManipulationOperatorType GetCollectionManipulationOperatorType(OperatorType @operator)
    {
        switch (@operator)
        {
            // Collection Manipulation Operators
            case OperatorType.Filter:
                return CollectionManipulationOperatorType.Filter;

            case OperatorType.Select:
                return CollectionManipulationOperatorType.Select;

            //case OperatorType.Transform:
            //    return CollectionManipulationOperatorType.Transform;

            case OperatorType.SelectMany:
                return CollectionManipulationOperatorType.SelectMany;

            case OperatorType.Limit:
                return CollectionManipulationOperatorType.Limit;

            case OperatorType.Skip:
                return CollectionManipulationOperatorType.Skip;
        }

        throw new InvalidOperationException();
    }

    public static CollectionAggregationOperatorType GetCollectionAggregationOperatorType(OperatorType @operator)
    {
        switch (@operator)
        {
            case OperatorType.Count:
                return CollectionAggregationOperatorType.Count;

            case OperatorType.Index:
                return CollectionAggregationOperatorType.Index;

            case OperatorType.Any:
                return CollectionAggregationOperatorType.Any;

            case OperatorType.All:
                return CollectionAggregationOperatorType.All;

            case OperatorType.Min:
                return CollectionAggregationOperatorType.Min;

            case OperatorType.Max:
                return CollectionAggregationOperatorType.Max;

            case OperatorType.Sum:
                return CollectionAggregationOperatorType.Sum;

            case OperatorType.Average:
                return CollectionAggregationOperatorType.Average;
        }

        throw new InvalidOperationException();
    }

    [Obsolete("WIP")]
    public static OperatorParametrizationType GetOperatorParametrization(OperatorType @operator)
    {
        switch (@operator)
        {
            case OperatorType.Not:
            //case OperatorType.Expr:
            case OperatorType.Type:
                return OperatorParametrizationType.Unary;

            case OperatorType.Add:
            case OperatorType.Subtract:
            case OperatorType.Divide:
            case OperatorType.Multiply:
            case OperatorType.Modulo:
            case OperatorType.Equals:
            case OperatorType.NotEquals:
            case OperatorType.Less:
            case OperatorType.LessEquals:
            case OperatorType.Greater:
            case OperatorType.GreaterEquals:
            case OperatorType.Like:
            case OperatorType.RegexMatch:
            //case OperatorType.Parse:
                return OperatorParametrizationType.Binary;

            case OperatorType.Filter:
            case OperatorType.Select:
            //case OperatorType.Transform:
                return OperatorParametrizationType.Predicate;

            case OperatorType.Or:
            case OperatorType.And:
                return OperatorParametrizationType.Array;

            case OperatorType.MemberAccess:
                break;

            case OperatorType.SelectMany:
                break;
            case OperatorType.Limit:
                break;
            case OperatorType.Skip:
                break;
            case OperatorType.Count:
            case OperatorType.Index:
            case OperatorType.Any:
            case OperatorType.All:
            case OperatorType.Min:
            case OperatorType.Max:
            case OperatorType.Sum:
            case OperatorType.Average:
                break;
        }

        throw new NotImplementedException();
    }

}
