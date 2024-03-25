using ModularSystem.Webql.Analysis.Semantics.Extensions;

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
    /// <summary>
    /// Visits an AST symbol and performs semantic analysis.
    /// </summary>
    /// <param name="context">The semantic analysis context containing the semantic table.</param>
    /// <param name="symbol">The symbol to visit and analyze.</param>
    /// <returns>The symbol, possibly with updated semantic information.</returns>
    public override Symbol Visit(SemanticContext context, Symbol symbol)
    {
        // Perform the base visit, which will recursively analyze child symbols first.
        symbol = base.Visit(context, symbol);

        // After visiting all children, we try to analyze the current symbol.
        var semantics = SemanticsAnalyser.TryAnalyse(context, symbol);

        // If semantic analysis was successful, associate the derived semantics with the symbol.
        if (semantics is not null)
        {
            symbol.AddSemantics(context, semantics);
        }

        return symbol;
    }
}
