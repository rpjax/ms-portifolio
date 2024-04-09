using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Extensions;

public static class SymbolExtensions
{
    public static string GetIdentifier(this ISymbol symbol)
    {
        if (symbol is IIdentifierSymbol identifierSymbol)
        {
            return identifierSymbol.Identifier;
        }

        return symbol.Hash;
    }

    public static T As<T>(this ISymbol symbol) where T : ISymbol
    {
        if (symbol is not T result)
        {
            throw new Exception();
        }

        return result;
    }

}
