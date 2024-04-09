using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Components;

/*
 * depth first, pre-order, traversal algorithm.
 */
public class AstTraverser
{
    protected bool SkipChildren { get; set; }
    protected bool Stop { get; set; }

    protected void TraverseTree(Symbol? symbol)
    {
        if(symbol is null)
        {
            return;
        }

        Visit(symbol);
        Stop = false;
        SkipChildren = false;
    }

    /*code that works on visiting the tree goes here, as override.*/
    protected virtual void OnVisit(Symbol symbol)
    {
        
    }

    protected virtual void AfterVisitChildren(Symbol symbol)
    {
    }

    private void Visit(Symbol symbol)
    {
        OnVisit(symbol);

        if (SkipChildren)
        {
            SkipChildren = false;
            return;
        }
        if (Stop)
        {
            return;
        }

        VisitChildren(symbol);
        AfterVisitChildren(symbol);
    }

    private void VisitChildren(Symbol symbol)
    {
        foreach (var child in symbol.Children)
        {
            if (Stop)
            {
                break;
            }

            Visit(child);
        }
    }

    private IEnumerable<Symbol> EnumerateTree(Symbol? symbol)
    {
        if(symbol is null)
        {
            yield break;
        }

        yield return symbol;

        foreach (var child in symbol.Children)
        {
            if(Stop)
            {
                Stop = false;
                yield break;
            }

            if(SkipChildren)
            {
                SkipChildren = false;
                continue;
            }

            foreach (var grandChild in EnumerateTree(child))
            {
                if (Stop)
                {
                    Stop = false;
                    yield break;
                }

                yield return grandChild;
            }
        }
    }
}
