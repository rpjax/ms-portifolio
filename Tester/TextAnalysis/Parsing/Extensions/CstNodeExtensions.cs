using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Extensions;

public static class CstNodeExtensions
{
    public static string ToHtmlTreeView(this CstNode node)
    {
        return new CstNodeHtmlBuilder(node)
            .Build();
    }

    public static CstRoot AsRoot(this CstNode node)
    {
        if(node is not CstRoot root)
        {
            throw new InvalidOperationException("Node is not a root node");
        }

        return root;
    }

    public static CstInternal AsInternal(this CstNode node)
    {
        if(node is not CstInternal internalNode)
        {
            throw new InvalidOperationException("Node is not an internal node");
        }

        return internalNode;
    }

    public static CstLeaf AsLeaf(this CstNode node)
    {
        if(node is not CstLeaf leaf)
        {
            throw new InvalidOperationException("Node is not a leaf node");
        }

        return leaf;
    }

}
