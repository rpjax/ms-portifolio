using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Synthesis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class ArgumentTranslator
{
    public Expression TranslateArgument(TranslationContext context, ArgumentSymbol arg)
    {
        if(arg is ReferenceSymbol reference)
        {
            return new ReferenceTranslator()
                .TranslateReference(context, reference);
        }

        if(arg is StatementBlockSymbol obj)
        {
            return new StatementBlockTranslator()
                .TranslateStatementBlock(context, obj); 
        }

        throw new TranslationException("", context);
    }

    public QueryArgumentExpression TranslateQueryArgument(TranslationContextOld context, ArgumentSymbol arg)
    {
        return new QueryArgumentExpression(TranslateQueryArgument(context, arg));
    }
}
