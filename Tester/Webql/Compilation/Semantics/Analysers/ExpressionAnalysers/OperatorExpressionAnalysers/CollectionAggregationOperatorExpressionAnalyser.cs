using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class CollectionAggregationOperatorExpressionAnalyzer
{
    public static OperatorExpressionSemantic AnalyzeCollectionAggregationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        //switch (OperatorHelper.GetCollectionAggregationOperatorType(symbol.Operator))
        //{
        //    case CollectionAggregationOperatorType.Count:
        //        return AnalyzeCountOperatorExpression(context, (CountOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Index:
        //        return AnalyzeIndexOperatorExpression(context, (IndexOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Any:
        //        return AnalyzeAnyOperatorExpression(context, (AnyOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.All:
        //        return AnalyzeAllOperatorExpression(context, (AllOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Min:
        //        return AnalyzeMinOperatorExpression(context, (MinOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Max:
        //        return AnalyzeMaxOperatorExpression(context, (MaxOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Sum:
        //        return AnalyzeSumOperatorExpression(context, (SumOperatorExpressionSymbol)symbol);

        //    case CollectionAggregationOperatorType.Average:
        //        return AnalyzeAverageOperatorExpression(context, (AverageOperatorExpressionSymbol)symbol);
        //}

        throw new NotImplementedException();
    }

    //public static OperatorExpressionSemantic AnalyzeCountOperatorExpression(
    //    SemanticContextOld context,
    //    CountOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeIndexOperatorExpression(
    //    SemanticContextOld context,
    //    IndexOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeAnyOperatorExpression(
    //    SemanticContextOld context,
    //    AnyOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeAllOperatorExpression(
    //    SemanticContextOld context,
    //    AllOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeMinOperatorExpression(
    //    SemanticContextOld context,
    //    MinOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeMaxOperatorExpression(
    //    SemanticContextOld context,
    //    MaxOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeSumOperatorExpression(
    //    SemanticContextOld context,
    //    SumOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}

    //public static OperatorExpressionSemantic AnalyzeAverageOperatorExpression(
    //    SemanticContextOld context,
    //    AverageOperatorExpressionSymbol symbol)
    //{
    //    // ...
    //}
}

