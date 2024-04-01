using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class CollectionManipulationOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseCollectionManipulationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        var operatorType = OperatorHelper.GetCollectionManipulationOperatorType(symbol.Operator);

        switch (operatorType)
        {
            case CollectionManipulationOperatorType.Filter:
                if (symbol is not FilterExpressionSymbol filterExpression)
                {
                    throw new Exception();
                }

                var source = filterExpression.Source;
                var sourceSemantic = SemanticAnalyser.AnalyseExpression(context, source);

                return new OperatorExpressionSemantic(
                    type: sourceSemantic.Type
                );

            case CollectionManipulationOperatorType.Select:
                break;
            case CollectionManipulationOperatorType.Transform:
                break;
            case CollectionManipulationOperatorType.SelectMany:
                break;
            case CollectionManipulationOperatorType.Limit:
                break;
            case CollectionManipulationOperatorType.Skip:
                break;
        }

        throw new InvalidOperationException();
    }
}

