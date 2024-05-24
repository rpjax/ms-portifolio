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
        //switch (OperatorHelper.GetCollectionAggregationOperatorType(symbol.Operator))
        //{
        //    case CollectionAggregationOperatorType.StatesCount:
        //        return AnalyseCountOperatorExpression(context, (CountOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Index:
        //        return AnalyseIndexOperatorExpression(context, (IndexOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Any:
        //        return AnalyseAnyOperatorExpression(context, (AnyOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.All:
        //        return AnalyseAllOperatorExpression(context, (AllOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Min:
        //        return AnalyseMinOperatorExpression(context, (MinOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Max:
        //        return AnalyseMaxOperatorExpression(context, (MaxOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Sum:
        //        return AnalyseSumOperatorExpression(context, (SumOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Average:
        //        return AnalyseAverageOperatorExpression(context, (AverageOperatorExpressionSymbol)symbol);
        //}

        throw new NotImplementedException();
    }

    //public static OperatorExpressionSemantic AnalyseCountOperatorExpression(
    //    SemanticContext context,
    //    CountOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseIndexOperatorExpression(
    //    SemanticContext context,
    //    IndexOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAnyOperatorExpression(
    //    SemanticContext context,
    //    AnyOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAllOperatorExpression(
    //    SemanticContext context,
    //    AllOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseMinOperatorExpression(
    //    SemanticContext context,
    //    MinOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseMaxOperatorExpression(
    //    SemanticContext context,
    //    MaxOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseSumOperatorExpression(
    //    SemanticContext context,
    //    SumOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAverageOperatorExpression(
    //    SemanticContext context,
    //    AverageOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}
}

