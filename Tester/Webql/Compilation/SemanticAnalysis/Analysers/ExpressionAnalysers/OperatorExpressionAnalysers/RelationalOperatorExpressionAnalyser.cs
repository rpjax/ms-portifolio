using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

/// <summary>
/// Provides functionality to analyse relational operator expressions.
/// </summary>
public static class RelationalOperatorExpressionAnalyser
{
    /// <summary>
    /// Analyses a relational operator expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The operator expression symbol to be analysed.</param>
    /// <returns>The semantic representation of the operator expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a relational operator expression symbol.</exception>
    public static OperatorExpressionSemantic AnalyseRelationalOperatorExpression(
      SemanticContext context,
      OperatorExpressionSymbol symbol)
    {
        if (symbol is not RelationalOperatorExpressionSymbol relationalExpression)
        {
            throw new Exception();
        }

        var leftSemantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(relationalExpression.LeftOperand),
            symbol: relationalExpression.LeftOperand
        );

        var rightSemantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(relationalExpression.RightOperand),
            symbol: relationalExpression.RightOperand
        );

        var leftType = leftSemantic.Type;
        var rightType = rightSemantic.Type;

        if (SemanticHelper.TypeIsNullable(leftType))
        {
            leftType = SemanticHelper.GetNullableUnderlyingType(leftType);
        }
        if (SemanticHelper.TypeIsNullable(rightType))
        {
            rightType = SemanticHelper.GetNullableUnderlyingType(rightType);
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
