using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Analysis.Semantics;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;

public static class SymbolExtensions
{
    public static T GetSemantic<T>(this Symbol symbol, TranslationContext context) where T : SymbolSemantic
    {
        return context.GetSemantics<T>(symbol);
    }
}
