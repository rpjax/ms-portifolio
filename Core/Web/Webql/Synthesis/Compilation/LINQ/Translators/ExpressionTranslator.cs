using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class ExpressionTranslator
{
    public Expression TranslateExpression(TranslationContext context, ExpressionSymbol arg)
    {
        switch (arg.ExpressionType)
        {
            case Analysis.Symbols.ExpressionType.Literal:
                break;
            case Analysis.Symbols.ExpressionType.Reference:
                break;
            case Analysis.Symbols.ExpressionType.Operator:
                break;
        }
    }
}
