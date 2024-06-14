using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

/// <summary>
/// Provides functionality to analyse operator expressions.
/// </summary>
public static class OperatorExpressionAnalyser
{
    /// <summary>
    /// Analyses an operator expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The operator expression symbol to be analysed.</param>
    /// <returns>The semantic representation of the operator expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a recognized operator expression symbol.</exception>
    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetOperatorSemanticType(symbol.Operator))
        {
            case OperatorSemanticType.Arithmetic:
                return AnalyseArithmeticOperatorExpression(context, symbol);

            case OperatorSemanticType.Relational:
                return AnalyseRelationalOperatorExpression(context, symbol);

            case OperatorSemanticType.StringRelational:
                return AnalyseStringRelationalOperatorExpression(context, symbol);

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

    /// <summary>
    /// Analyses an arithmetic operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseArithmeticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return ArithmeticOperatorExpressionAnalyser.AnalyseArithmeticOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a relational operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseRelationalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return RelationalOperatorExpressionAnalyser.AnalyseRelationalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a string relational operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseStringRelationalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return StringRelationalOperatorExpressionAnalyser.AnalyseStringRelationalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a logical operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseLogicalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return LogicalOperatorExpressionAnalyser.AnalyseLogicalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a semantic operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseSemanticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return SemanticOperatorExpressionAnalyser.AnalyseSemanticOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a collection manipulation operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseCollectionManipulationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionManipulationOperatorExpressionAnalyser.AnalyseCollectionManipulationOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a collection aggregation operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseCollectionAggregationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionAggregationOperatorExpressionAnalyser.AnalyseCollectionAggregationOperatorExpression(context, symbol);
    }
}
