using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class ReferenceExpressionAnalyser
{
    public static ReferenceExpressionSemantic AnalyseReferenceExpression(
        SemanticContext context,
        ReferenceExpressionSymbol symbol)
    {
        var identifier = symbol.GetNormalizedValue();
        var semantic = context.GetDeclarationSemantic(identifier);

        return new ReferenceExpressionSemantic(
            type: semantic.Type
        );
    }
}
