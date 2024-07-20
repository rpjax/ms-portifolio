namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public class CstNodeVisitor
{
    public virtual CstNode Visit(CstNode node)
    {
        return node.Accept(this);
    }

    public virtual CstNode VisitRoot(CstRoot node)
    {
        return node;
    }

    public virtual CstNode VisitNonTerminal(CstInternal node)
    {
        return node;
    }

    public virtual CstNode VisitTerminal(CstLeaf node)
    {
        return node;
    }

}
