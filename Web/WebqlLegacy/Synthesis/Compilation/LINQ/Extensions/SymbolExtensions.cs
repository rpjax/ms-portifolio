using Aidan.Webql.Analysis;
using Aidan.Webql.Analysis.Semantics;

namespace Aidan.Webql.Synthesis.Compilation.LINQ.Extensions;

public static class SymbolExtensions
{
    public static T GetSemantic<T>(this Symbol symbol, TranslationContext context) where T : SymbolSemantic
    {
        return context.GetSemantics<T>(symbol);
    }
}
