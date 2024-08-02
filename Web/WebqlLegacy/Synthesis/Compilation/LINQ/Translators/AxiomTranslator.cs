using Aidan.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis.Compilation.LINQ;

public class AxiomTranslator
{
    public Expression TranslateAxiom(TranslationContext context, AxiomSymbol symbol)
    {
        if(symbol.Lambda is null)
        {
            throw new NotImplementedException();
        }

        return new LambdaTranslator()
            .TranslateLambda(context, symbol.Lambda);
    }
}
