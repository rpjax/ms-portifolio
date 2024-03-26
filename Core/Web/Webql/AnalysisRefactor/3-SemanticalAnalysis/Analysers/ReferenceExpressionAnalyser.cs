using ModularSystem.Webql.Analysis.Semantics.Analysers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public class ReferenceExpressionAnalyser : SemanticAnalyserBase
{
    public ReferenceExpressionSemantic AnalyseReferenceExpression(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        var identifier = symbol.ToString();
        var referencedSymbol = context.GetDeclaration<ExpressionSymbol>(identifier);
        var semantic = context.GetSemantic<ExpressionSemantic>(referencedSymbol);
        var type = semantic.Type;

        return new ReferenceExpressionSemantic(
            type: type
        );
    }
}