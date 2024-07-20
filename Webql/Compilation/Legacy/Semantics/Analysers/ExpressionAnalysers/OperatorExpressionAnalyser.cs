using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

/// <summary>
/// Provides functionality to Analyze operator expressions.
/// </summary>
public static class OperatorExpressionAnalyzer
{
    /// <summary>
    /// Analyzes an operator expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The operator expression symbol to be Analyzed.</param>
    /// <returns>The semantic representation of the operator expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a recognized operator expression symbol.</exception>
    public static OperatorExpressionSemantic AnalyzeOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetOperatorSemanticType(symbol.Operator))
        {
            case OperatorSemanticType.Arithmetic:
                return AnalyzeArithmeticOperatorExpression(context, symbol);

            case OperatorSemanticType.Relational:
                return AnalyzeRelationalOperatorExpression(context, symbol);

            case OperatorSemanticType.StringRelational:
                return AnalyzeStringRelationalOperatorExpression(context, symbol);

            case OperatorSemanticType.Logical:
                return AnalyzeLogicalOperatorExpression(context, symbol);

            case OperatorSemanticType.Semantic:
                return AnalyzeSemanticOperatorExpression(context, symbol);

            case OperatorSemanticType.CollectionManipulation:
                return AnalyzeCollectionManipulationOperatorExpression(context, symbol);

            case OperatorSemanticType.CollectionAggregation:
                return AnalyzeCollectionAggregationOperatorExpression(context, symbol);
        }

        throw new Exception();
    }

    /// <summary>
    /// Analyzes an arithmetic operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeArithmeticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return ArithmeticOperatorExpressionAnalyzer.AnalyzeArithmeticOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a relational operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeRelationalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return RelationalOperatorExpressionAnalyzer.AnalyzeRelationalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a string relational operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeStringRelationalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return StringRelationalOperatorExpressionAnalyzer.AnalyzeStringRelationalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a logical operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeLogicalOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return LogicalOperatorExpressionAnalyzer.AnalyzeLogicalOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a semantic operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeSemanticOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return SemanticOperatorExpressionAnalyzer.AnalyzeSemanticOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a collection manipulation operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeCollectionManipulationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionManipulationOperatorExpressionAnalyzer.AnalyzeCollectionManipulationOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a collection aggregation operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeCollectionAggregationOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return CollectionAggregationOperatorExpressionAnalyzer.AnalyzeCollectionAggregationOperatorExpression(context, symbol);
    }
}
