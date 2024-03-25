using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class StatementBlockAnalyser
{
    public StatementBlockSemantics AnalyseStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        return new StatementBlockSemantics(
            resolvedType: null
        );
    }
}
