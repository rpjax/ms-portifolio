using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class LogicalOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseLogicalOperatorExpression(
      SemanticContext context,
      OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetLogicalOperatorType(symbol.Operator))
        {
            case LogicalOperatorType.Or:
                return AnalyseOrOperatorExpression(context, (OrOperatorExpressionSymbol)symbol);

            case LogicalOperatorType.And:
                return AnalyseAndOperatorExpression(context, (AndOperatorExpressionSymbol)symbol);

            case LogicalOperatorType.Not:
                return AnalyseNotOperatorExpression(context, (NotOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyseOrOperatorExpression(
        SemanticContext context,
        OrOperatorExpressionSymbol symbol)
    {
        foreach (var expression in symbol.Expressions)
        {
            var semantic = SemanticAnalyser.AnalyseExpression(context, expression);

            if (semantic.Type != typeof(bool))
            {
                throw new InvalidOperationException();
            }
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static OperatorExpressionSemantic AnalyseAndOperatorExpression(
        SemanticContext context,
        AndOperatorExpressionSymbol symbol)
    {
        foreach (var expression in symbol.Expressions)
        {
            var semantic = SemanticAnalyser.AnalyseExpression(context, expression);

            if (semantic.Type != typeof(bool))
            {
                throw new InvalidOperationException();
            }
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static OperatorExpressionSemantic AnalyseNotOperatorExpression(
        SemanticContext context,
        NotOperatorExpressionSymbol symbol)
    {
        var semantic = SemanticAnalyser.AnalyseExpression(context, symbol.Operand);

        if (semantic.Type != typeof(bool))
        {
            throw new InvalidOperationException();
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

}
