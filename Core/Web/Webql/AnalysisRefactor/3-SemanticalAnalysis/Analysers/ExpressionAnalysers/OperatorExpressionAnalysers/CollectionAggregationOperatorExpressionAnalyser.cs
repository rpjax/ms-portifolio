using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class CollectionAggregationOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseCollectionAggregationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetCollectionAggregationOperatorType(symbol.Operator))
        {
            case CollectionAggregationOperatorType.Count:
                break;
            case CollectionAggregationOperatorType.Index:
                break;
            case CollectionAggregationOperatorType.Any:
                break;
            case CollectionAggregationOperatorType.All:
                break;
            case CollectionAggregationOperatorType.Min:
                break;
            case CollectionAggregationOperatorType.Max:
                break;
            case CollectionAggregationOperatorType.Sum:
                break;
            case CollectionAggregationOperatorType.Average:
                break;
        }

        throw new NotImplementedException();
    }



}

