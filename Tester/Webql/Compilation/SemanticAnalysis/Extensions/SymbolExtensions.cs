using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class SymbolExtensions
{
    public static T As<T>(this ISymbol symbol, SemanticContext context) where T : ISymbol
    {
        if (symbol is not T result)
        {
            throw new Exception();
        }

        return result;
    }
}
