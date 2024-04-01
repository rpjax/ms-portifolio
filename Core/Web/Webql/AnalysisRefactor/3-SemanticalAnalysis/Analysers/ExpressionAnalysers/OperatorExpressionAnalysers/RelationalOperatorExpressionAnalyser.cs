using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class RelationalOperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseRelationalOperatorExpression(
      SemanticContext context,
      OperatorExpressionSymbol symbol)
    {
        return new OperatorExpressionSemantic(
            type: typeof(bool)
        );
    }
}
