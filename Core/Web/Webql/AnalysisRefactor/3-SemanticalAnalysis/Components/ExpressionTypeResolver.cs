﻿using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class ExpressionTypeResolver
{       
    public static Type ResolveType(SemanticContext context, LiteralExpressionSymbol symbol)
    {
        switch (symbol.LiteralType)
        {
            case LiteralType.Null:
                return typeof(void);

            case LiteralType.String:
                return typeof(string);

            case LiteralType.Bool:
                return typeof(bool);

            case LiteralType.Number:
                return typeof(int);
        }

        throw new InvalidOperationException();
    }

    public static Type ResolveType(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        var identifier = symbol.ToString();
        var semantic = context.GetDeclarationSemantic(identifier);

        return semantic.Type;
    }

    public static Type ResolveType(SemanticContext context, OperatorExpressionSymbol symbol)
    {
        switch (symbol.Operator)
        {
            case Symbols.OperatorType.Add:
                break;
            case Symbols.OperatorType.Subtract:
                break;
            case Symbols.OperatorType.Divide:
                break;
            case Symbols.OperatorType.Multiply:
                break;
            case Symbols.OperatorType.Modulo:
                break;
            case Symbols.OperatorType.Equals:
                break;
            case Symbols.OperatorType.NotEquals:
                break;
            case Symbols.OperatorType.Less:
                break;
            case Symbols.OperatorType.LessEquals:
                break;
            case Symbols.OperatorType.Greater:
                break;
            case Symbols.OperatorType.GreaterEquals:
                break;
            case Symbols.OperatorType.Like:
                break;
            case Symbols.OperatorType.RegexMatch:
                break;
            case Symbols.OperatorType.Or:
                break;
            case Symbols.OperatorType.And:
                break;
            case Symbols.OperatorType.Not:
                break;
            case Symbols.OperatorType.Expr:
                break;
            case Symbols.OperatorType.Parse:
                break;
            case Symbols.OperatorType.SelectOld:
                break;
            case Symbols.OperatorType.AnonymousType:
                break;
            case Symbols.OperatorType.Filter:
                break;
            case Symbols.OperatorType.Select:
                break;
            case Symbols.OperatorType.Transform:
                break;
            case Symbols.OperatorType.SelectMany:
                break;
            case Symbols.OperatorType.Limit:
                break;
            case Symbols.OperatorType.Skip:
                break;
            case Symbols.OperatorType.Count:
                break;
            case Symbols.OperatorType.Index:
                break;
            case Symbols.OperatorType.Any:
                break;
            case Symbols.OperatorType.All:
                break;
            case Symbols.OperatorType.Min:
                break;
            case Symbols.OperatorType.Max:
                break;
            case Symbols.OperatorType.Sum:
                break;
            case Symbols.OperatorType.Average:
                break;
        }

        throw new NotImplementedException();
        //return Type.GetType(symbol.Typ)
    }
}
