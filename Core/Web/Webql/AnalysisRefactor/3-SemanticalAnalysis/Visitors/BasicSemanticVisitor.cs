using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

public class BasicSemanticVisitor : AstSemanticVisitor
{
    protected override DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        //* creates the semantics object.
        var semantic = SemanticAnalyser.AnalyseDeclaration(context, symbol); ;

        //* binds the semantics object to the symbol.
        symbol.AddSemantic(context, semantic);

        //* declares the symbol.
        symbol.AddDeclaration(context, symbol.Identifier);

        return base.VisitDeclaration(context, symbol);
    }
}
