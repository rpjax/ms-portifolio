using System.Runtime.CompilerServices;
using Webql.Parsing.Tools;

namespace Webql.Parsing.Components;

/// <summary>
/// Represents a node in the abstract syntax tree for the WebQL language.
/// </summary>
public abstract class WebqlSyntaxNode
{
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
    protected abstract Dictionary<string, object> Attributes { get; }

    public WebqlSyntaxNode()
    {
    }

    /// <summary>
    /// Accepts a visitor to traverse the abstract syntax tree and perform modifications or analysis.
    /// </summary>
    /// <param name="visitor"></param>
    public abstract WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasAttribute(string key)
    {
        return Attributes.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? TryGetAttribute(string key)
    {
        if (Attributes.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetAttribute(string key, object value)
    {
        Attributes[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAttribute(string key)
    {
        Attributes.Remove(key);
    }
}

public static class WebqlSyntaxNodeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? TryGetAttribute<T>(this WebqlSyntaxNode node, string key)
    {
        var attribute = node.TryGetAttribute(key);

        if (attribute is T value)
        {
            return value;
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddAttribute(this WebqlSyntaxNode node, string key, object value)
    {
        if(node.HasAttribute(key))
        {
            throw new InvalidOperationException($"The attribute '{key}' already exists on the node.");
        }

        node.SetAttribute(key, value);
    }
}
