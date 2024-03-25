using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class ReferenceTranslator
{
    public Expression TranslateReference(TranslationContext context, ReferenceSymbol symbol)
    {
        return context.GetTranslationTableEntry(symbol.Value).Expression;
    }
}
