using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class StatementBlockAnalyzer
{
    public static StatementBlockSemantic AnalyzeStatementBlock(SemanticContextOld context, StatementBlockSymbol symbol)
    {
        var lastStatement = symbol.LastOrDefault();
        var type = typeof(void);

        if (lastStatement is ExpressionSymbol expression)
        {
            var semantic = SemanticAnalyzer.AnalyzeExpression(
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
