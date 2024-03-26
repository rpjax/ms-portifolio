using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class ExpressionTypeResolver
{       
    public static Type ResolveType(SemanticContext context, LiteralExpressionSymbol symbol)
    {
        throw new Exception();
    }

    public static Type ResolveType(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        return context.GetSemantic<ReferenceExpressionSemantic>(symbol).Type;
    }

    public static Type ResolveType(SemanticContext context, OperatorExpressionSymbol symbol)
    {
        switch (symbol.Operator)
        {
            case ExpressionOperator.Add:
                break;
            case ExpressionOperator.Subtract:
                break;
            case ExpressionOperator.Divide:
                break;
            case ExpressionOperator.Multiply:
                break;
            case ExpressionOperator.Modulo:
                break;
            case ExpressionOperator.Equals:
                break;
            case ExpressionOperator.NotEquals:
                break;
            case ExpressionOperator.Less:
                break;
            case ExpressionOperator.LessEquals:
                break;
            case ExpressionOperator.Greater:
                break;
            case ExpressionOperator.GreaterEquals:
                break;
            case ExpressionOperator.Like:
                break;
            case ExpressionOperator.RegexMatch:
                break;
            case ExpressionOperator.Or:
                break;
            case ExpressionOperator.And:
                break;
            case ExpressionOperator.Not:
                break;
            case ExpressionOperator.Expr:
                break;
            case ExpressionOperator.Parse:
                break;
            case ExpressionOperator.Select:
                break;
            case ExpressionOperator.Type:
                break;
            case ExpressionOperator.Filter:
                break;
            case ExpressionOperator.Project:
                break;
            case ExpressionOperator.Transform:
                break;
            case ExpressionOperator.SelectMany:
                break;
            case ExpressionOperator.Limit:
                break;
            case ExpressionOperator.Skip:
                break;
            case ExpressionOperator.Count:
                break;
            case ExpressionOperator.Index:
                break;
            case ExpressionOperator.Any:
                break;
            case ExpressionOperator.All:
                break;
            case ExpressionOperator.Min:
                break;
            case ExpressionOperator.Max:
                break;
            case ExpressionOperator.Sum:
                break;
            case ExpressionOperator.Average:
                break;
        }

        throw new Exception();
        //return Type.GetType(symbol.Typ)
    }
}
