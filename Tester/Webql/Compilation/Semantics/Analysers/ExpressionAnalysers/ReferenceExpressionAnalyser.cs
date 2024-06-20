using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class ReferenceExpressionAnalyzer
{
    public static ReferenceExpressionSemantic AnalyzeReferenceExpression(
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
