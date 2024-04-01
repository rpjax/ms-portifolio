using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class ArithmeticOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseArithmeticOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        if (symbol is not BinaryExpressionSymbol binaryExpression)
        {
            throw new Exception();
        }

        var leftSemantic = SemanticAnalyser.AnalyseExpression(context, binaryExpression.LeftOperand);
        var rightSemantic = SemanticAnalyser.AnalyseExpression(context, binaryExpression.RightOperand);

        var leftType = leftSemantic.Type;
        var rightType = rightSemantic.Type;

        if (!SemanticHelper.TypeIsNumber(leftType))
        {
            throw new Exception();
        }
        if (!SemanticHelper.TypeIsNumber(rightType))
        {
            throw new Exception();
        }

        var anyOperandIsFloat = SemanticHelper.TypeIsFloatNumber(leftType) || SemanticHelper.TypeIsFloatNumber(rightType);



        return new OperatorExpressionSemantic(
            type: null
        );
    }
}
