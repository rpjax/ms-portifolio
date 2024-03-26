using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class ExpressionSymbolExtensions
{
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

            default:
                break;
        }

        throw new InvalidOperationException();
    }
}
