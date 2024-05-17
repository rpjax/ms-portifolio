using System.Collections;
using System.Linq.Expressions;
using System.Text;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a node in a concrete syntax tree.
/// </summary>
public class CstNode : IEnumerable<CstNode>
{
    public string Identifier { get; }
    public IReadOnlyList<string> Lexemes { get; }
    public CstNode? Parent { get; internal set; }

    internal List<CstNode> InternalChildren { get; }

    public CstNode(string id, string[] lexemes, CstNode? parent, CstNode[] children)
    {
        Identifier = id;
        Lexemes = lexemes;
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
        var tabs = new string(' ', level * 4);  // Generate the indentation for the current level.
        var childIndent = new string(' ', (level + 1) * 4); // Indentation for child elements.

        var builder = new StringBuilder();
        builder.AppendLine($"{tabs}<{Identifier}>");

        if (Lexemes.Any())
        {
            // Add lexemes with indentation, followed by a newline if there are child elements.
            builder.AppendLine($"{childIndent}{string.Join(" ", Lexemes)}");
        }

        // Recursively call ToString on each child with increased indentation level.
        foreach (var child in Children)
        {
            builder.AppendLine(child.ToString(level + 1));
        }

        builder.Append($"{tabs}</{Identifier}>");
        return builder.ToString();
    }

    public CstNode? Reduce(CstMaskCollection masks)
    {
        var mask = masks.TryGetMask(Identifier);

        if (mask is null)
        {
            return this;
        }

        if (Identifier != mask.Identifier)
        {
            throw new InvalidOperationException($"Cannot reduce node with identifier '{Identifier}' using mask '{mask.Identifier}'.");
        }

        var lexemes = Lexemes.Where(lexeme => !mask.LexemeBlacklist.Contains(lexeme)).ToArray();

        var children = Children
            .Select(child => child.Reduce(masks))
            .Where(child => child is not null)
            .Select(child => child!)
            .ToArray();

        if (lexemes.Length == 0 && children.Length == 0)
        {
            return null;
        }

        return new CstNode(Identifier, lexemes, Parent, children);
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
            lexemes: Attributes.ToArray(),
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


public class CstNodeMask
{
    public string Identifier { get; }
    public string[] LexemeBlacklist { get; }

    public CstNodeMask(string id, string[] lexemeBlacklist)
    {
        Identifier = id;
        LexemeBlacklist = lexemeBlacklist;
    }

    public CstNodeMask(string id, Func<CstNode, CstNode> reducer)
    {

    }

}

public class CstMaskCollection : IEnumerable<CstNodeMask>
{
    private CstNodeMask[] Masks { get; }

    public CstMaskCollection(params CstNodeMask[] masks)
    {
        Masks = masks;
    }

    public CstNodeMask? TryGetMask(string identifier)
    {
        return Masks.FirstOrDefault(mask => mask.Identifier == identifier);
    }

    public IEnumerator<CstNodeMask> GetEnumerator()
    {
        return ((IEnumerable<CstNodeMask>)Masks).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
