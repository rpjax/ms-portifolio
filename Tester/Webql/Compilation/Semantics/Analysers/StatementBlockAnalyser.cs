using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class StatementBlockAnalyser
{
    public static StatementBlockSemantic AnalyseStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        var lastStatement = symbol.LastOrDefault();
        var type = typeof(void);

        if (lastStatement is ExpressionSymbol expression)
        {
            var semantic = SemanticAnalyser.AnalyseExpression(
                context: context.GetSymbolContext(expression),
                symbol: expression
            );

            type = semantic.Type;
        }

        return new StatementBlockSemantic(
            resolvedType: type
        );
    }
}
