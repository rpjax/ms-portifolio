using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class ReferenceTranslator
{
    public Expression TranslateReference(TranslationContext context, ReferenceExpressionSymbol symbol)
    {
        return context.GetTranslationTableEntry(symbol.GetNormalizedValue()).Expression;
    }
}
