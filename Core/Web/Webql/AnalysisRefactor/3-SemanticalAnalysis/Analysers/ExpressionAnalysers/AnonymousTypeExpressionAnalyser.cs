using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class AnonymousTypeExpressionAnalyser
{
    public static TypeProjectionExpressionSemantic AnalyseAnonymousTypeExpression(
        SemanticContext context,
        AnonymousTypeExpressionSymbol symbol)
    {
        return new TypeProjectionExpressionSemantic(
            type: null
        );
    }
}
