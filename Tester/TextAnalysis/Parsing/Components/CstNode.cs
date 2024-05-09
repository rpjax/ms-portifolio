using System.Collections;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a node in a concrete syntax tree.
/// </summary>
public class CstNode : IEnumerable<CstNode>
{
    public string Identifier { get; }
    public IReadOnlyList<string> Attributes { get; }
    public CstNode? Parent { get; internal set; }

    internal List<CstNode> InternalChildren { get; }

    public CstNode(string id, string[] attributes, CstNode? parent, CstNode[] children)
    {
        Identifier = id;
        Attributes = attributes;
        Parent = parent;
        InternalChildren = new(children);
    }

    public IReadOnlyList<CstNode> Children => InternalChildren;
    public bool IsRoot => Parent is null;
    public bool IsLeaf => Children.Count == 0;

    public IEnumerator<CstNode> GetEnumerator()
    {
        return InternalChildren.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return ToString(0);
    }

    private string ToString(int level)
    {
        var @base = $"{GetTabs(level)}{Identifier}: [{string.Join(" ", Attributes)}]";

        if (IsLeaf)
        {
            return @base;
        }

        var childrenStr = string.Join(Environment.NewLine, Children.Select(c => c.ToString(level + 1)));

        return $"{@base}{Environment.NewLine}{childrenStr}";
    }

    private string GetTabs(int level)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < level; i++)
        {
            builder.Append("    ");
        }

        return builder.ToString();
    }

}

/// <summary>
/// Represents a builder for a <see cref="CstNode"/>.
/// </summary>
public class CstNodeBuilder
{
    private string Identifier { get; }
    private List<string> Attributes { get; }
    private List<CstNode> Children { get; }
    private List<CstNodeBuilder> ChildBuilders { get; }

    public CstNodeBuilder(string id)
    {
        Identifier = id;
        Attributes = new();
        Children = new();
        ChildBuilders = new();
    }

    public CstNodeBuilder AddAttribute(string attribute)
    {
        Attributes.Add(attribute);
        return this;
    }

    public CstNodeBuilder AddChild(CstNode child)
    {
        Children.Add(child);
        return this;
    }

    public CstNodeBuilder AddChildBuilder(CstNodeBuilder childBuilder)
    {
        ChildBuilders.Add(childBuilder);
        return this;
    }

    public CstNode Build()
    {
        var parent = new CstNode(
            id: Identifier,
            attributes: Attributes.ToArray(),
            parent: null,
            children: Children.ToArray()
        );

        foreach (var child in Children)
        {
            child.SetParent(parent);
        }

        return parent;
    }
}
