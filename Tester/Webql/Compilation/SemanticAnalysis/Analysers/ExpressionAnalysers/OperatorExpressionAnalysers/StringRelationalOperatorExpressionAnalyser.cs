using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class StringRelationalOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseStringRelationalOperatorExpression(
      SemanticContext context,
      OperatorExpressionSymbol symbol)
    {
        if (symbol is not StringRelationalOperatorExpressionSymbol strRelationalExpression)
        {
            throw new Exception();
        }

        var leftSemantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(strRelationalExpression.LeftOperand),
            symbol: strRelationalExpression.LeftOperand
        );

        var rightSemantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(strRelationalExpression.RightOperand),
            symbol: strRelationalExpression.RightOperand
        );

        var leftType = leftSemantic.Type;
        var rightType = rightSemantic.Type;

        if (leftType != typeof(string))
        {
            throw new Exception();
        }
        if (rightType != typeof(string))
        {
            throw new Exception();
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }
}

