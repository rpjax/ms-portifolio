using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class StatementBlockTranslator
{
    public Expression TranslateStatementBlock(TranslationContext context, StatementBlockSymbol symbol)
    {
        var expr = null as Expression;
        var translator = new StatementTranslator();

        foreach (var statement in symbol)
        {
            var _expr = translator.TranslateStatement(context, statement);

            if (_expr is not null)
            {
                expr = _expr;
            }
        }

        if(expr is null)
        {
            throw new Exception();
        }

        return expr;
    }
}
