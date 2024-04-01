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
public enum OperatorReturnStuff
{
    Arithmetic,
    Relational,
    Logical
}

public static class OperatorHelper
{
    public static OperatorParametrizationType GetParametrization(OperatorTypeOld operatorType)
    {
        switch (operatorType)
        {
            case OperatorTypeOld.Not:
            case OperatorTypeOld.Select:
            case OperatorTypeOld.Expr:
            case OperatorTypeOld.Type:
                return OperatorParametrizationType.Unary;

            case OperatorTypeOld.Add:
            case OperatorTypeOld.Subtract:
            case OperatorTypeOld.Divide:
            case OperatorTypeOld.Multiply:
            case OperatorTypeOld.Modulo:
            case OperatorTypeOld.Equals:
            case OperatorTypeOld.NotEquals:
            case OperatorTypeOld.Less:
            case OperatorTypeOld.LessEquals:
            case OperatorTypeOld.Greater:
            case OperatorTypeOld.GreaterEquals:
            case OperatorTypeOld.Like:
            case OperatorTypeOld.RegexMatch:
            case OperatorTypeOld.Parse:
                return OperatorParametrizationType.Binary;

            case OperatorTypeOld.Filter:
            case OperatorTypeOld.Project:
            case OperatorTypeOld.Transform:
                return OperatorParametrizationType.Predicate;

            case OperatorTypeOld.Or:
            case OperatorTypeOld.And:
                return OperatorParametrizationType.Array;

            case OperatorTypeOld.MemberAccess:
                break;

            case OperatorTypeOld.SelectMany:
                break;
            case OperatorTypeOld.Limit:
                break;
            case OperatorTypeOld.Skip:
                break;
            case OperatorTypeOld.Count:
            case OperatorTypeOld.Index:
            case OperatorTypeOld.Any:
            case OperatorTypeOld.All:
            case OperatorTypeOld.Min:
            case OperatorTypeOld.Max:
            case OperatorTypeOld.Sum:
            case OperatorTypeOld.Average:
                break;
        }
    }

    public static IEnumerable<Symbols.OperatorType> GetUnaryOperators()
    {
        yield return OperatorTypeOld.Not;
        yield return OperatorTypeOld.Select;
        yield return OperatorTypeOld.Type;
    }

    public static IEnumerable<Symbols.OperatorType> GetBinaryOperators()
    {
        return Enumerable.Empty<OperatorTypeOld>()
            .Concat(GetArithmeticOperators())
            .Concat(GetRelationalOperators())
            .Concat(GetPatternMatchOperators())
            ;
    }

    public static IEnumerable<Symbols.OperatorType> GetArithmeticOperators()
    {
        yield return OperatorTypeOld.Add;
        yield return OperatorTypeOld.Subtract;
        yield return OperatorTypeOld.Divide;
        yield return OperatorTypeOld.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetRelationalOperators()
    {
        yield return OperatorTypeOld.Add;
        yield return OperatorTypeOld.Subtract;
        yield return OperatorTypeOld.Divide;
        yield return OperatorTypeOld.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetPatternMatchOperators()
    {
        yield return OperatorTypeOld.Like;
        yield return OperatorTypeOld.RegexMatch;
    }

    public static IEnumerable<Symbols.OperatorType> GetLogicalOperators()
    {
        yield return OperatorTypeOld.Or;
        yield return OperatorTypeOld.And;
        yield return OperatorTypeOld.Not;
    }

    public static IEnumerable<Symbols.OperatorType> GetSemanticOperators()
    {
        yield return OperatorTypeOld.Expr;
        yield return OperatorTypeOld.Parse;
        yield return OperatorTypeOld.Select;
        yield return OperatorTypeOld.Type;
        yield return OperatorTypeOld.MemberAccess;
    }

    public static IEnumerable<Symbols.OperatorType> GetQueryOperators()
    {
        yield return OperatorTypeOld.Filter;
        yield return OperatorTypeOld.Project;
        yield return OperatorTypeOld.Transform;
        yield return OperatorTypeOld.SelectMany;
        yield return OperatorTypeOld.Limit;
        yield return OperatorTypeOld.Skip;
    }

    public static IEnumerable<Symbols.OperatorType> GetAggregationOperators()
    {
        yield return OperatorTypeOld.Count;
        yield return OperatorTypeOld.Index;
        yield return OperatorTypeOld.Any;
        yield return OperatorTypeOld.All;
        yield return OperatorTypeOld.Min;
        yield return OperatorTypeOld.Max;
        yield return OperatorTypeOld.Sum;
        yield return OperatorTypeOld.Average;
    }
} 
