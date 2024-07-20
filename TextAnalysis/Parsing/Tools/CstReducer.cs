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
    public CstRoot Execute()
    {
        var rootName = Root.Name;
        var rootChildren = Reduce(Root);
        var rootMetadata = Root.Metadata;

        while (true)
        {
            var shouldReduceRoot = rootChildren.Length == 1 
                && !NonTerminalWhitelist.Contains(Root.Name);

            if (shouldReduceRoot && rootChildren[0] is CstInternal internalNode)
            {
                rootName = internalNode.Name;
                rootChildren = internalNode.Children;
                continue;
            }

            break;
        }

        return new CstRoot(rootName, rootChildren, rootMetadata);
    }

    private CstNode[] Reduce(CstNode node)
    {
        switch (node.Type)
        {
            case CstNodeType.Root:
                return ReduceMany(((CstRoot)node).Children);

            case CstNodeType.Internal:
                return ReduceInternal((CstInternal)node);

            case CstNodeType.Leaf:
                return ReduceLeaf((CstLeaf)node);

            default:
                throw new InvalidOperationException();
        }
    }

    private CstNode[] ReduceInternal(CstInternal node)
    {
        if (!NonTerminalWhitelist.Contains(node.Name))
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
            new CstInternal(node.Name, newChildren.ToArray(), node.Metadata)
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
