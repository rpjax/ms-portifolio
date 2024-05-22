using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Tools;

public class CstReducer
{
    private CstNode Root { get; }
    private string[] NonTerminalWhitelist { get; }

    public CstReducer(CstNode root, string[] nonTerminalWhitelist)
    {
        Root = root;
        NonTerminalWhitelist = nonTerminalWhitelist;
    }

    public CstNode ReduceRoot()
    {
        if(Root is not CstNonTerminal nonTerminal)
        {
            throw new InvalidOperationException();
        }

        var newChildren = Reduce(nonTerminal);

        return new CstNonTerminal(
            nonTerminal.Symbol, 
            newChildren
        );
    }

    private CstNode[] Reduce(CstNode node)
    {
        switch (node.Type)
        {
            case CstNodeType.Terminal:
                return ReduceTerminal((CstTerminal)node);

            case CstNodeType.NonTerminal:
                return ReduceNonTerminal((CstNonTerminal)node);

            case CstNodeType.Epsilon:
                return Array.Empty<CstNode>();

            default:
                throw new InvalidOperationException();
        }
    }

    private CstNode[] ReduceNonTerminal(CstNonTerminal nonTerminal)
    {
        if (!NonTerminalWhitelist.Contains(nonTerminal.Symbol.Name))
        {
            return ReduceChildren(nonTerminal);
        }

        var children = nonTerminal.Children;
        var newChildren = new List<CstNode>();

        foreach (var child in children)
        {
            newChildren.AddRange(Reduce(child));
        }

        return new CstNonTerminal[]
        {
            new CstNonTerminal(nonTerminal.Symbol, newChildren.ToArray())
        };
    }

    private CstNode[] ReduceTerminal(CstTerminal terminal)
    {
        return new CstNode[] { terminal };
    }

    private CstNode[] ReduceChildren(CstNonTerminal nonTerminal)
    {
        var children = nonTerminal.Children;
        var newChildren = new List<CstNode>();

        foreach (var child in children)
        {
            newChildren.AddRange(Reduce(child));
        }

        return newChildren.ToArray();
    }

}
