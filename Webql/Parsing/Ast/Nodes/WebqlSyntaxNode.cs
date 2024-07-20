using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Webql.Parsing.Analysis;

namespace Webql.Parsing.Ast;

/// <summary>
/// Represents a node in the abstract syntax tree for the WebQL language.
/// </summary>
public abstract class WebqlSyntaxNode
{
    /// <summary>
    /// Gets the parent node of the current node in the abstract syntax tree.
    /// </summary>
    public WebqlSyntaxNode? Parent { get; internal set; }

    /// <summary>
    /// Gets the type of the node. It is used to determine the kind of node before casting.
    /// </summary>
    public abstract WebqlNodeType NodeType { get; }

    /// <summary>
    /// Gets the syntax metadata associated with the node.
    /// </summary>
    public abstract WebqlSyntaxNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the attributes associated with the node.
    /// </summary>
    public abstract Dictionary<string, object> Attributes { get; }

    public WebqlSyntaxNode()
    {
    }

    /// <summary>
    /// Accepts a visitor to traverse the abstract syntax tree and perform modifications or analysis.
    /// </summary>
    /// <param name="visitor"></param>
    public abstract WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor);

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<WebqlSyntaxNode> GetChildren();

    /// <summary>
    /// Returns a string representation of the node.
    /// </summary>
    /// <returns></returns>
    public abstract override string ToString();

    /// <summary>
    /// Binds the parent node to the children nodes.
    /// </summary>
    public void BindChildren()
    {
        foreach (var child in GetChildren())
        {
            child.Parent = this;
            child.BindChildren();
        }
    }
}

public static class WebqlSyntaxNodeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAttribute(this WebqlSyntaxNode node, string key)
    {
        return node.Attributes.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetAttribute(this WebqlSyntaxNode node, string key, [MaybeNullWhen(false)] out object value)
    {
        return node.Attributes.TryGetValue(key, out value);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetAttribute<T>(this WebqlSyntaxNode node, string key, [MaybeNullWhen(false)] out T value)
    {
        if (node.Attributes.TryGetValue(key, out var obj))
        {
            if(obj is T t)
            {
                value = t;
                return true;
            }
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? TryGetAttribute<T>(this WebqlSyntaxNode node, string key)
    {
        if (node.Attributes.TryGetValue(key, out var obj) && obj is T t)
        {
            return t;
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetAttribute<T>(this WebqlSyntaxNode node, string key)
    {
        if (node.Attributes.TryGetValue(key, out var obj))
        {
            if(obj is not T t)
            {
                throw new InvalidOperationException($"The attribute '{key}' is not of type '{typeof(T).Name}'.");
            }

            return t;
        }

        throw new InvalidOperationException($"The attribute '{key}' does not exist on the node.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetAttribute(this WebqlSyntaxNode node, string key, object value)
    {
        node.Attributes[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAttribute(this WebqlSyntaxNode node, string key)
    {
        node.Attributes.Remove(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddAttribute(this WebqlSyntaxNode node, string key, object value)
    {
        if (node.HasAttribute(key))
        {
            throw new InvalidOperationException($"The attribute '{key}' already exists on the node.");
        }

        node.SetAttribute(key, value);
    }
}
