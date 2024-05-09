namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public static class CstNodeExtensions
{
    internal static void SetParent(this CstNode self, CstNode parent)
    {
        self.Parent = parent;
    }

    internal static void AddChild(this CstNode self, CstNode child)
    {
        self.InternalChildren.Add(child);
    }

    public static CstNodeBuilder ToBuilder(this CstNode node)
    {
        var builder = new CstNodeBuilder(node.Identifier);

        foreach (var attribute in node.Attributes)
        {
            builder.AddAttribute(attribute);
        }

        foreach (var child in node.Children)
        {
            builder.AddChild(child);
        }

        return builder;
    }

}