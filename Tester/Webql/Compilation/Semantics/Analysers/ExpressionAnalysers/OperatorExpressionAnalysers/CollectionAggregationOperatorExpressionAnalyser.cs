using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class CollectionAggregationOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseCollectionAggregationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        //switch (OperatorHelper.GetCollectionAggregationOperatorType(symbol.Operator))
        //{
        //    case CollectionAggregationOperatorType.Count:
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
    //    SemanticContextOld context,
    //    CountOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseIndexOperatorExpression(
    //    SemanticContextOld context,
    //    IndexOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAnyOperatorExpression(
    //    SemanticContextOld context,
    //    AnyOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAllOperatorExpression(
    //    SemanticContextOld context,
    //    AllOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseMinOperatorExpression(
    //    SemanticContextOld context,
    //    MinOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseMaxOperatorExpression(
    //    SemanticContextOld context,
    //    MaxOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseSumOperatorExpression(
    //    SemanticContextOld context,
    //    SumOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyseAverageOperatorExpression(
    //    SemanticContextOld context,
    //    AverageOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}
}

