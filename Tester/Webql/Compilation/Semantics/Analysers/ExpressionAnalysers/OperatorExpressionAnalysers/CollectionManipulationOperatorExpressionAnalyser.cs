using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class CollectionManipulationOperatorExpressionAnalyzer
{
    public static OperatorExpressionSemantic AnalyzeCollectionManipulationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetCollectionManipulationOperatorType(symbol.Operator))
        {
            case CollectionManipulationOperatorType.Filter:
                return AnalyzeFilterOperatorExpression(context, (FilterOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.Select:
                return AnalyzeSelectOperatorExpression(context, (SelectOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.SelectMany:
                throw new NotImplementedException();

            case CollectionManipulationOperatorType.Limit:
                return AnalyzeLimitOperatorExpression(context, (LimitOperatorExpressionSymbol)symbol);

            case CollectionManipulationOperatorType.Skip:
                return AnalyzeSkipOperatorExpression(context, (SkipOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyzeFilterOperatorExpression(
        SemanticContextOld context,
        FilterOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;

        var sourceSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(source),
            symbol: source
        );

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyzeSelectOperatorExpression(
        SemanticContextOld context,
        SelectOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;
        var lambda = symbol.Lambda;

        var sourceSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(source),
            symbol: source
        );

        var lambdaSemantic = SemanticAnalyzer.AnalyzeLambda(
            context: context.GetSymbolContext(lambda),
            symbol: lambda
        );

        var type = sourceSemantic
            .GetQueryableGenericType(context)
            .MakeGenericType(lambdaSemantic.ReturnType);

        return new OperatorExpressionSemantic(
            type: type
        );
    }

    public static OperatorExpressionSemantic AnalyzeLimitOperatorExpression(
       SemanticContextOld context,
       LimitOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;

        var sourceSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(source),
            symbol: source
        );

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

    public static OperatorExpressionSemantic AnalyzeSkipOperatorExpression(
       SemanticContextOld context,
       SkipOperatorExpressionSymbol symbol)
    {
        var source = symbol.Source;

        var sourceSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(source),
            symbol: source
        );

        return new OperatorExpressionSemantic(
            type: sourceSemantic.Type
        );
    }

}

