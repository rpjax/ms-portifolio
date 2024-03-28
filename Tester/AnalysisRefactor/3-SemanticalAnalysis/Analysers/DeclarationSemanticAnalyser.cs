using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class DeclarationSemanticAnalyser : SemanticAnalyserBase
{
    public DeclarationStatementSemantic AnalyseDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        if(symbol.Type is null)
        {
            throw new Exception();
        }

        return new DeclarationStatementSemantic(
            type: SemanticAnalyser.GetType(context, symbol.Type)
        );
    }

    public DeclarationStatementSemantic[] AnalyseDeclarations(SemanticContext context, DeclarationStatementSymbol[] symbols)
    {
        return symbols
            .Select(x => SemanticAnalyser.AnalyseDeclaration(context, x))
            .ToArray();
    }
}
