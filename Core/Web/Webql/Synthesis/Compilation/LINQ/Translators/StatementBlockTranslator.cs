using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class StatementBlockTranslator
{
    public Expression TranslateStatementBlock(TranslationContext context, StatementBlockSymbol obj)
    {
        var expr = null as Expression;
        var translator = new StatementTranslator();

        foreach (var item in obj)
        {
            expr = translator.TranslateStatement(context, item);
        }

        if(expr is null)
        {
            throw new Exception();
        }

        return expr;
    }
}
