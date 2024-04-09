using ModularSystem.Webql.Analysis.Components;
using ModularSystem.Webql.Analysis.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Components;

public class AstSemanticRewriter : AstRewriter
{
    protected SemanticContext Context { get; set; }

    public AstSemanticRewriter(SemanticContext context)
    {
        Context = context;
    }

    protected override void OnVisit(Symbol symbol)
    {
        if (symbol is IScopeSymbol scopeSymbol)
        {
            Context.EnterScope(scopeSymbol.GetIdentifier());
        }

        OnSemanticVisit(symbol);
    }

    protected override void AfterVisitChildren(Symbol symbol)
    {
        if (symbol is IScopeSymbol)
        {
            Context.ExitScope();
        }
    }

    protected virtual void OnSemanticVisit(Symbol symbol)
    {
    }
}
