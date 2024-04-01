using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class CollectionAggregationOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseCollectionAggregationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }
}

