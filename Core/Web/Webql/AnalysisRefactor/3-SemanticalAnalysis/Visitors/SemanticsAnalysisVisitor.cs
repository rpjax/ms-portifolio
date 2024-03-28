using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

/// <summary>
/// The SemanticsAnalysisVisitor traverses the AST, generating semantic information for each node.
/// This visitor pattern ensures that semantic analysis occurs in a post-order traversal manner,
/// meaning that it first visits all descendant nodes before the node itself. This order is crucial
/// because it allows for the semantic information to bubble up from the leaves to the root of the AST.
/// Each node's semantic information is then stored in the SemanticTable for subsequent phases.
/// </summary>
public class SemanticsAnalysisVisitor : AstSemanticVisitor
{
    public void Run(Symbol symbol)
    {
        var context = new SemanticContext();
        Visit(context, symbol);
    }

    /// <summary>
    /// Visits an AST symbol and performs semantic analysis.
    /// </summary>
    /// <param name="context">The semantic analysis context containing the semantic table.</param>
    /// <param name="symbol">The symbol to visit and analyze.</param>
    /// <returns>The symbol, possibly with updated semantic information.</returns>
    protected override Symbol Visit(SemanticContext context, Symbol symbol)
    {
        // After visiting all children, we try to analyze the current symbol.
        var semantics = SemanticAnalyser.TryAnalyse(context, symbol);

        // If semantic analysis was successful, associate the derived semantics with the symbol.
        if (semantics is not null)
        {
            symbol.AddSemantic(context, semantics);
        }

        // Perform the base visit, which will recursively analyze child symbols first.
        return base.Visit(context, symbol);
    }

    protected override DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        symbol.AddDeclaration(context, symbol.Identifier);
        return base.VisitDeclaration(context, symbol);
    }
}
