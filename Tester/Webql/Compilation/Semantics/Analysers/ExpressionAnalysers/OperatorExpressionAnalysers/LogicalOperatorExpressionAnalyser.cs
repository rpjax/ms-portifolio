using ModularSystem.Webql.Analysis.Semantics.Helpers;
using ModularSystem.Webql.Analysis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class LogicalOperatorExpressionAnalyzer
{
    public static OperatorExpressionSemantic AnalyzeLogicalOperatorExpression(
      SemanticContextOld context,
      OperatorExpressionSymbol symbol)
    {
        switch (OperatorHelper.GetLogicalOperatorType(symbol.Operator))
        {
            case LogicalOperatorType.Or:
                return AnalyzeOrOperatorExpression(context, (OrOperatorExpressionSymbol)symbol);

            case LogicalOperatorType.And:
                return AnalyzeAndOperatorExpression(context, (AndOperatorExpressionSymbol)symbol);

            case LogicalOperatorType.Not:
                return AnalyzeNotOperatorExpression(context, (NotOperatorExpressionSymbol)symbol);
        }

        throw new InvalidOperationException();
    }

    public static OperatorExpressionSemantic AnalyzeOrOperatorExpression(
        SemanticContextOld context,
        OrOperatorExpressionSymbol symbol)
    {
        foreach (var expression in symbol.Expressions)
        {
            var semantic = SemanticAnalyzer.AnalyzeExpression(
                context: context.GetSymbolContext(expression), 
                symbol: expression
            );

            if (semantic.Type != typeof(bool))
            {
                throw new InvalidOperationException();
            }
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static OperatorExpressionSemantic AnalyzeAndOperatorExpression(
        SemanticContextOld context,
        AndOperatorExpressionSymbol symbol)
    {
        foreach (var expression in symbol.Expressions)
        {
            var semantic = SemanticAnalyzer.AnalyzeExpression(
                context: context.GetSymbolContext(expression),
                symbol: expression
            );

            if (semantic.Type != typeof(bool))
            {
                throw new InvalidOperationException();
            }
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static OperatorExpressionSemantic AnalyzeNotOperatorExpression(
        SemanticContextOld context,
        NotOperatorExpressionSymbol symbol)
    {
        var semantic = SemanticAnalyzer.AnalyzeExpression(
            context: context.GetSymbolContext(symbol.Operand),
            symbol: symbol.Operand
        );

        if (semantic.Type != typeof(bool))
        {
            throw new InvalidOperationException();
        }

        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }

}
