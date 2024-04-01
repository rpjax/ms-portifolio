using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class OperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetOperatorSemanticType(symbol.Operator))
        {
            case OperatorSemanticType.Arithmetic:
                return AnalyseArithmeticOperatorExpression(context, symbol);

            case OperatorSemanticType.Relational:
                return AnalyseRelationalOperatorExpression(context, symbol);

            case OperatorSemanticType.Logical:
                return AnalyseLogicalOperatorExpression(context, symbol);

            case OperatorSemanticType.Semantic:
                return AnalyseSemanticOperatorExpression(context, symbol);

            case OperatorSemanticType.CollectionManipulation:
                return AnalyseCollectionManipulationOperatorExpression(context, symbol);

            case OperatorSemanticType.CollectionAggregation:
                return AnalyseCollectionAggregationOperatorExpression(context, symbol);
        }

        throw new Exception();
    }

    public static OperatorExpressionSemantic AnalyseArithmeticOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return ArithmeticOperatorExpressionAnalyser.AnalyseArithmeticOperatorExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseRelationalOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return RelationalOperatorExpressionAnalyser.AnalyseRelationalOperatorExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseLogicalOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return LogicalOperatorExpressionAnalyser.AnalyseLogicalOperatorExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseSemanticOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return SemanticOperatorExpressionAnalyser.AnalyseSemanticOperatorExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseCollectionManipulationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionManipulationOperatorExpressionAnalyser.AnalyseCollectionManipulationOperatorExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseCollectionAggregationOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionAggregationOperatorExpressionAnalyser.AnalyseCollectionAggregationOperatorExpression(context, symbol);
    }
}
