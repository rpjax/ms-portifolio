using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class ExpressionSymbolExtensions
{
    [Obsolete("Resolve expressions is impossible before semantic analysis. Use ExpressionSemanticExtensions instead.")]
    public static Type ResolveType(this ExpressionSymbol symbol, SemanticContext context)
    {
        throw new InvalidOperationException();
    }

}
