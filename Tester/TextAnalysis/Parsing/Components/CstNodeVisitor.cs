namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public class CstNodeVisitor
{
    public virtual CstNode Visit(CstNode node)
    {
        return node.Accept(this);
    }

    public virtual CstTerminal VisitTerminal(CstTerminal node)
    {
        return node;
    }

    public virtual CstNonTerminal VisitNonTerminal(CstNonTerminal node)
    {
        return node;
    }

    public virtual CstEpsilon VisitEpsilon(CstEpsilon node)
    {
        return node;
    }

}
