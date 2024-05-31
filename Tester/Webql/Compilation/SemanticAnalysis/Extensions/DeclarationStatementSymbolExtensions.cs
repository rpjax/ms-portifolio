using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class DeclarationStatementSymbolExtensions
{
    public static void SetType(this DeclarationStatementSymbol symbol, string type)
    {
        symbol.Type = type;
    }

}