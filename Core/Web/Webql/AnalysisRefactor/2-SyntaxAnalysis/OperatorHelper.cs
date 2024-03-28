using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Syntax;

public static class OperatorHelper
{
    public static IEnumerable<Symbols.OperatorType> GetUnaryOperators()
    {
        yield return Symbols.OperatorType.Not;
        yield return Symbols.OperatorType.Select;
        yield return Symbols.OperatorType.Type;
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
        yield return Symbols.OperatorType.Add;
        yield return Symbols.OperatorType.Subtract;
        yield return Symbols.OperatorType.Divide;
        yield return Symbols.OperatorType.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetRelationalOperators()
    {
        yield return Symbols.OperatorType.Add;
        yield return Symbols.OperatorType.Subtract;
        yield return Symbols.OperatorType.Divide;
        yield return Symbols.OperatorType.Multiply;
    }

    public static IEnumerable<Symbols.OperatorType> GetPatternMatchOperators()
    {
        yield return Symbols.OperatorType.Like;
        yield return Symbols.OperatorType.RegexMatch;
    }

    public static IEnumerable<Symbols.OperatorType> GetLogicalOperators()
    {
        yield return Symbols.OperatorType.Or;
        yield return Symbols.OperatorType.And;
        yield return Symbols.OperatorType.Not;
    }

    public static IEnumerable<Symbols.OperatorType> GetSemanticOperators()
    {
        yield return Symbols.OperatorType.Expr;
        yield return Symbols.OperatorType.Parse;
        yield return Symbols.OperatorType.Select;
        yield return Symbols.OperatorType.Type;
        yield return Symbols.OperatorType.MemberAccess;
    }

    public static IEnumerable<Symbols.OperatorType> GetQueryOperators()
    {
        yield return Symbols.OperatorType.Filter;
        yield return Symbols.OperatorType.Project;
        yield return Symbols.OperatorType.Transform;
        yield return Symbols.OperatorType.SelectMany;
        yield return Symbols.OperatorType.Limit;
        yield return Symbols.OperatorType.Skip;
    }

    public static IEnumerable<Symbols.OperatorType> GetAggregationOperators()
    {
        yield return Symbols.OperatorType.Count;
        yield return Symbols.OperatorType.Index;
        yield return Symbols.OperatorType.Any;
        yield return Symbols.OperatorType.All;
        yield return Symbols.OperatorType.Min;
        yield return Symbols.OperatorType.Max;
        yield return Symbols.OperatorType.Sum;
        yield return Symbols.OperatorType.Average;
    }
} 
