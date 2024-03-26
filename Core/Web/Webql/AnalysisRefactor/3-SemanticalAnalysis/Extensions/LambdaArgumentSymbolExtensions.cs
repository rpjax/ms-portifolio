using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class LambdaArgumentSymbolExtensions
{
    public static void SetType(this LambdaArgumentSymbol symbol, string type)
    {
        symbol.Type = type;
    }

}