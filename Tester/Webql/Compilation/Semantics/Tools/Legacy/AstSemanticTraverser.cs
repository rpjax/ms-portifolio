using ModularSystem.Webql.Analysis.Components;
using ModularSystem.Webql.Analysis.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Components;

/// <summary>
/// Represents a traverser that performs semantic analysis on an AST.
/// </summary>
public abstract class AstSemanticTraverser : AstTraverser
{
    /// <summary>
    /// Gets the semantic context used during the traversal.
    /// </summary>
    protected SemanticContextOld Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AstSemanticTraverser"/> class.
    /// </summary>
    /// <param name="context">The semantic context to use during the traversal.</param>
    protected AstSemanticTraverser(SemanticContextOld context)
    {
        Context = context;
    }

    /// <summary>
    /// Called when a symbol is visited. If the symbol is a scope symbol, enters the scope.
    /// </summary>
    /// <param name="symbol">The symbol being visited.</param>
    protected override void OnVisit(Symbol symbol)
    {
        if (symbol is IScopeSymbol scopeSymbol)
        {
            Context.EnterScope(scopeSymbol.GetIdentifier());
        }

        OnSemanticVisit(symbol);
    }

    /// <summary>
    /// Called after the children of a symbol have been visited. If the symbol is a scope symbol, exits the scope.
    /// </summary>
    /// <param name="symbol">The symbol whose children have been visited.</param>
    protected override void AfterVisitChildren(Symbol symbol)
    {
        if (symbol is IScopeSymbol)
        {
            Context.ExitScope();
        }
    }

    /// <summary>
    /// Called when a symbol is visited during semantic analysis. Can be overridden in subclasses to perform custom actions.
    /// </summary>
    /// <param name="symbol">The symbol being visited.</param>
    protected virtual void OnSemanticVisit(Symbol symbol)
    {
    }

}
