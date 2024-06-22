using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class DeclarationAnalyzer
{
    public static DeclarationStatementSemantic AnalyzeDeclaration(SemanticContextOld context, IDeclarationSymbol symbol)
    {
        if(symbol.Type is null)
        {
            throw new Exception();
        }

        return new DeclarationStatementSemantic(
            type: SemanticAnalyzer.GetType(context, symbol.Type)
        );
    }

    public static DeclarationStatementSemantic[] AnalyzeDeclarations(SemanticContextOld context, IDeclarationSymbol[] symbols)
    {
        return symbols
            .Select(x => SemanticAnalyzer.AnalyzeDeclaration(context.GetSymbolContext(x), x))
            .ToArray();
    }
}
