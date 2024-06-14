using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class ReferenceExpressionAnalyser
{
    public static ReferenceExpressionSemantic AnalyseReferenceExpression(
        SemanticContextOld context,
        ReferenceExpressionSymbol symbol)
    {
        var identifier = symbol.GetNormalizedValue();
        var type = context.GetSymbolType(identifier);

        return new ReferenceExpressionSemantic(
            type: type
        );
    }
}
