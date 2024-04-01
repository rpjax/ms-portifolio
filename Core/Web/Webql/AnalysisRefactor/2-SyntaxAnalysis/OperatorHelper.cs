using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Syntax;

public enum OperatorParametrizationType
{
    Specific,
    Unary,
    Binary,
    Predicate,
    Array
}

//*
// NOTE: idea for an enum that describes what type of value the operator produces,
// ex: logical operators always resolve to boolean, and their operands also have to be boolean.
// Maybe this could help in the semantic analysis.
//*
public enum OperatorSemanticType
{
    Arithmetic,
    Relational,
    Logical,
    Semantic,
    CollectionManipulation,
    CollectionAggregation
}

public static class OperatorHelper
{
    public static OperatorParametrizationType GetParametrization(OperatorType @operator)
    {
        switch (@operator)
        {
            case OperatorType.Not:
            case OperatorType.Select:
            case OperatorType.Expr:
            case OperatorType.AnonymousType:
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
            case OperatorType.Parse:
                return OperatorParametrizationType.Binary;

            case OperatorType.Filter:
            case OperatorType.Project:
            case OperatorType.Transform:
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
            case OperatorType.Like:
            case OperatorType.RegexMatch:
                return OperatorSemanticType.Relational;

            // Logical Operators
            case OperatorType.Or:
            case OperatorType.And:
            case OperatorType.Not:
                return OperatorSemanticType.Logical;

            // Semantic Operators
            case OperatorType.Expr:
            case OperatorType.Parse:
            case OperatorType.Select:
            case OperatorType.AnonymousType:
            case OperatorType.MemberAccess:
                return OperatorSemanticType.Semantic;

            // Collection Manipulation Operators
            case OperatorType.Filter:
            case OperatorType.Project:
            case OperatorType.Transform:
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

    public static IEnumerable<Symbols.OperatorType> GetUnaryOperators()
    {
        yield return OperatorType.Not;
        yield return OperatorType.Select;
        yield return OperatorType.AnonymousType;
    }

    public static IEnumerable<Symbols.OperatorType> GetBinaryOperators()
    {
        return Enumerable.Empty<OperatorType>()
            .Concat(GetArithmeticOperators())
            .Concat(GetRelationalOperators())
            .Concat(GetPatternMatchOperators())
            ;
    }

    public static IEnumerable<Symbols.OperatorType> GetArithmeticOperators()
    {
        yield return OperatorType.Add;
        yield return OperatorType.Subtract;
        yield return OperatorType.Divide;
        yield return OperatorType.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetRelationalOperators()
    {
        yield return OperatorType.Add;
        yield return OperatorType.Subtract;
        yield return OperatorType.Divide;
        yield return OperatorType.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetPatternMatchOperators()
    {
        yield return OperatorType.Like;
        yield return OperatorType.RegexMatch;
    }

    public static IEnumerable<Symbols.OperatorType> GetLogicalOperators()
    {
        yield return OperatorType.Or;
        yield return OperatorType.And;
        yield return OperatorType.Not;
    }

    public static IEnumerable<Symbols.OperatorType> GetSemanticOperators()
    {
        yield return OperatorType.Expr;
        yield return OperatorType.Parse;
        yield return OperatorType.Select;
        yield return OperatorType.AnonymousType;
        yield return OperatorType.MemberAccess;
    }

    public static IEnumerable<Symbols.OperatorType> GetQueryOperators()
    {
        yield return OperatorType.Filter;
        yield return OperatorType.Project;
        yield return OperatorType.Transform;
        yield return OperatorType.SelectMany;
        yield return OperatorType.Limit;
        yield return OperatorType.Skip;
    }

    public static IEnumerable<Symbols.OperatorType> GetAggregationOperators()
    {
        yield return OperatorType.Count;
        yield return OperatorType.Index;
        yield return OperatorType.Any;
        yield return OperatorType.All;
        yield return OperatorType.Min;
        yield return OperatorType.Max;
        yield return OperatorType.Sum;
        yield return OperatorType.Average;
    }
} 
