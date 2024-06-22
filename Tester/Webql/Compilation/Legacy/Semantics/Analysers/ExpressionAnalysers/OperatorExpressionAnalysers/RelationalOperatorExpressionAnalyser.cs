using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

/// <summary>
/// Provides functionality to Analyze relational operator expressions.
/// </summary>
public static class RelationalOperatorExpressionAnalyzer
{
    /// <summary>
    /// Analyzes a relational operator expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The operator expression symbol to be Analyzed.</param>
    /// <returns>The semantic representation of the operator expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a relational operator expression symbol.</exception>
    public static OperatorExpressionSemantic AnalyzeRelationalOperatorExpression(
      SemanticContextOld context,
      OperatorExpressionSymbol symbol)
    {
        if (symbol is not RelationalOperatorExpressionSymbol relationalExpression)
        {
            throw new Exception();
        }

        var leftSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(relationalExpression.LeftOperand),
            symbol: relationalExpression.LeftOperand
        );

        var rightSemantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(relationalExpression.RightOperand),
            symbol: relationalExpression.RightOperand
        );

        var leftType = leftSemantic.Type;
        var rightType = rightSemantic.Type;

        if (SemanticsHelper.TypeIsNullable(leftType))
        {
            leftType = SemanticsHelper.GetNullableUnderlyingType(leftType);
        }
        if (SemanticsHelper.TypeIsNullable(rightType))
        {
            rightType = SemanticsHelper.GetNullableUnderlyingType(rightType);
        }

        if (leftType != rightType)
        {
            throw new Exception();
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }
}
