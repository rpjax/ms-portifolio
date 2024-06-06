using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the type of a node in the concrete syntax tree (CST).
/// </summary>
public enum CstNodeType
{
    Root,
    Internal,
    Leaf,
}

///// <summary>
///// Represents a node in the concrete syntax tree (CST).
///// </summary>
//public interface ICstNode
//{
//    /// <summary>
//    /// Gets the type of the node.
//    /// </summary>
//    CstNodeType ExpressionType { get; }

//    /// <summary>
//    /// Gets or sets a property by key.
//    /// </summary>
//    /// <param name="key"></param>
//    /// <returns></returns>
//    object this[string key] { get; set; }
//}

///// <summary>
///// Represents a root node in the concrete syntax tree (CST).
///// </summary>
//public interface ICstRoot : ICstInternal
//{

//}

///// <summary>
///// Represents an internal node in the concrete syntax tree (CST).
///// </summary>
//public interface ICstInternal : ICstNode
//{
//    string Name { get; }
//    ICstNode[] Children { get; }
//    bool IsEpsilon { get; }
//}

///// <summary>
///// Represents a leaf node in the concrete syntax tree (CST).
///// </summary>
//public interface ICstLeaf : ICstNode
//{
//    /// <summary>
//    /// Gets the token associated with the leaf node.
//    /// </summary>
//    Token Token { get; }
//}

/// <summary>
/// Represents a node in the concrete syntax tree (CST).
/// </summary>
public abstract class CstNode 
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public abstract CstNodeType Type { get; }

    /// <summary>
    /// Gets the name associated with the node.
    /// </summary>
    public abstract string Name { get; } 

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public abstract CstNodeMetadata Metadata { get; }

    private Dictionary<string, object> Properties { get; } = new();

    /// <summary>
    /// Gets or sets a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[string key]
    {
        get => Properties[key];
        set => Properties[key] = value;
    }

    /// <summary>
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract CstNode Accept(CstNodeVisitor visitor);

    /// <summary>
    /// Gets the properties of the node.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, object>> GetProperties()
    {
        return Properties;
    }

    /// <summary>
    /// Gets a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetProperty(string key)
    {
        return Properties[key];
    }

    /// <summary>
    /// Tries to get a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object? TryGetProperty(string key)
    {
        return Properties.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Sets a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetProperty(string key, object value)
    {
        Properties[key] = value;
    }
}

/// <summary>
/// Represents a root node in the concrete syntax tree (CST).
/// </summary>
public class CstRoot : CstNode
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public override CstNodeType Type => CstNodeType.Root;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the children of the root node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstRoot"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="children"></param>
    /// <param name="metadata"></param>
    public CstRoot(string name, CstNode[] children, CstNodeMetadata metadata) 
    {
        Name = name;
        Children = children;
        Metadata = metadata;
    }

    /// <summary>
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitRoot(this);
    }
}

/// <summary>
/// Represents an internal node in the concrete syntax tree (CST).
/// </summary>
public class CstInternal : CstNode
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public override CstNodeType Type => CstNodeType.Internal;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the children of the internal node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Gets a value indicating whether the internal node is an epsilon node.
    /// </summary>
    public bool IsEpsilon { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstInternal"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="children"></param>
    /// <param name="metadata"></param>
    /// <param name="isEpsilon"></param>
    public CstInternal(string name, CstNode[] children, CstNodeMetadata metadata, bool isEpsilon = false) 
    {
        Name = name;
        Children = children;
        Metadata = metadata;
        IsEpsilon = isEpsilon;
    }

    /// <summary>
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitNonTerminal(this);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return Name;
    }
}

/// <summary>
/// Represents a leaf node in the concrete syntax tree (CST).
/// </summary>
public class CstLeaf : CstNode
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public override CstNodeType Type => CstNodeType.Leaf;

    /// <inheritdoc/>
    public override string Name { get => Token.Type.ToString(); }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the token associated with the leaf node.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstLeaf"/> class.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="metadata"></param>
    public CstLeaf(Token token, CstNodeMetadata metadata)
    {
        Token = token;
        Metadata = metadata;
    }

    ///<inheritdoc/>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitTerminal(this);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return Token.Value.ToString();
    }

}
