using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class LogicalOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseLogicalOperatorExpression(
      SemanticContext context,
      OperatorExpressionSymbol symbol)
    {
        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }
}
