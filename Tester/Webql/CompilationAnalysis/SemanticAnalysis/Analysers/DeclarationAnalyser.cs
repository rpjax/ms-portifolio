﻿using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class DeclarationAnalyser
{
    public static DeclarationStatementSemantic AnalyseDeclaration(SemanticContext context, IDeclarationSymbol symbol)
    {
        if(symbol.Type is null)
        {
            throw new Exception();
        }

        return new DeclarationStatementSemantic(
            type: SemanticAnalyser.GetType(context, symbol.Type)
        );
    }

    public static DeclarationStatementSemantic[] AnalyseDeclarations(SemanticContext context, IDeclarationSymbol[] symbols)
    {
        return symbols
            .Select(x => SemanticAnalyser.AnalyseDeclaration(context.GetSymbolContext(x), x))
            .ToArray();
    }
}
