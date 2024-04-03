using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class CollectionManipulationOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseCollectionManipulationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetCollectionManipulationOperatorType(symbol.Operator))
        {
            case CollectionManipulationOperatorType.Filter:
                return AnalyseFilterOperatorExpression(context, (FilterOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.Select:
                return AnalyseSelectOperatorExpression(context, (SelectOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.Transform:
                break;

            case CollectionManipulationOperatorType.SelectMany:
                break;

            case CollectionManipulationOperatorType.Limit:
                return AnalyseLimitOperatorExpression(context, (LimitOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.Skip:
                return AnalyseSkipOperatorExpression(context, (SkipOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyseFilterOperatorExpression(
        SemanticContext context,
        FilterOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;
        var sourceSemantic = SemanticAnalyser.AnalyseExpression(context, source);

        if (sourceSemantic.IsNotQueryable(context))
        {
            throw new Exception();
        }

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyseSelectOperatorExpression(
        SemanticContext context,
        SelectOperatorExpressionSymbol symbol)
    {
        throw new NotImplementedException();
    }

    public static OperatorExpressionSemantic AnalyseLimitOperatorExpression(
       SemanticContext context,
       LimitOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;
        var sourceSemantic = SemanticAnalyser.AnalyseExpression(context, source);

        if (sourceSemantic.IsNotQueryable(context))
        {
            throw new Exception();
        }

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyseSkipOperatorExpression(
       SemanticContext context,
       SkipOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;
        var sourceSemantic = SemanticAnalyser.AnalyseExpression(context, source);

        if (sourceSemantic.IsNotQueryable(context))
        {
            throw new Exception();
        }

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

}

