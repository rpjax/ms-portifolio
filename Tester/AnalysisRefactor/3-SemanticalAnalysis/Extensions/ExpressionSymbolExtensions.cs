﻿using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class ExpressionSymbolExtensions
{
    [Obsolete("Resolve expressions is impossible before semantic analysis. Use ExpressionSemanticExtensions instead.")]
    public static Type ResolveType(this ExpressionSymbol symbol, SemanticContext context)
    {
        switch (symbol.ExpressionType)
        {
            case ExpressionType.Literal:
                return ExpressionTypeResolver.ResolveType(context, (LiteralExpressionSymbol)symbol);

            case ExpressionType.Reference:
                return ExpressionTypeResolver.ResolveType(context, (ReferenceExpressionSymbol)symbol);

            case ExpressionType.Operator:
                return ExpressionTypeResolver.ResolveType(context, (OperatorExpressionSymbol)symbol);

            case ExpressionType.Lambda:
                break;

            case ExpressionType.AnonymousType:
                break;
        }

        throw new InvalidOperationException();
    }

}
