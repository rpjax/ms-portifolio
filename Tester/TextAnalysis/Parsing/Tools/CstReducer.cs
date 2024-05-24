using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Tools;

/// <summary>
/// Defines a class that reduces a concrete syntax tree (CST) to a more compact form.
/// </summary>
public class CstReducer
{
    private CstRoot Root { get; }
    private string[] NonTerminalWhitelist { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstReducer"/> class.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="nonTerminalWhitelist"></param>
    public CstReducer(CstRoot root, string[] nonTerminalWhitelist)
    {
        Root = root;
        NonTerminalWhitelist = nonTerminalWhitelist;
    }

    /// <summary>
    /// Reduces the concrete syntax tree (CST) to a more compact form.
    /// </summary>
    /// <returns></returns>
    public CstRoot ReduceCst()
    {
        return new CstRoot(
            Root.Symbol,
            Reduce(Root)
        );
    }

    private CstNode[] Reduce(CstNode node)
    {
        switch (node.Type)
        {
            case CstNodeType.Root:
                return ReduceRoot((CstRoot)node);

            case CstNodeType.Internal:
                return ReduceInternal((CstInternal)node);

            case CstNodeType.Leaf:
                return ReduceLeaf((CstLeaf)node);

            default:
                throw new InvalidOperationException();
        }
    }

    private CstNode[] ReduceRoot(CstRoot node)
    {
        if (node.Symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException();
        }

        if (!NonTerminalWhitelist.Contains(nonTerminal.Name))
        {
            return ReduceMany(node.Children);
        }

        var newChildren = new List<CstNode>();

        foreach (var child in node.Children)
        {
            newChildren.AddRange(Reduce(child));
        }

        return new CstInternal[]
        {
            new CstInternal(nonTerminal, newChildren.ToArray())
        };
    }

    private CstNode[] ReduceInternal(CstInternal node)
    {
        if(node.Symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException();
        }

        if (!NonTerminalWhitelist.Contains(nonTerminal.Name))
        {
            return ReduceMany(node.Children);
        }

        var newChildren = new List<CstNode>();

        foreach (var child in node.Children)
        {
            newChildren.AddRange(Reduce(child));
        }

        return new CstInternal[]
        {
            new CstInternal(nonTerminal, newChildren.ToArray())
        };
    }

    private CstNode[] ReduceLeaf(CstLeaf leaf)
    {
        return new CstNode[] { leaf };
    }

    private CstNode[] ReduceMany(CstNode[] children)
    {
        var newChildren = new List<CstNode>();

        foreach (var child in children)
        {
            newChildren.AddRange(Reduce(child));
        }

        return newChildren.ToArray();
    }

}
