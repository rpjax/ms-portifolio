using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Analysis.Semantics;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;

public static class SymbolExtensions
{
    public static T GetSemantics<T>(this Symbol symbol, TranslationContext context) where T : SymbolSemantics
    {
        return context.GetSemantics<T>(symbol);
    }
}
