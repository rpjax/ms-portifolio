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
    /// Gets or sets a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[string key]
    {
        get => Properties[key];
        set => Properties[key] = value;
    }

    private Dictionary<string, object> Properties { get; } = new();

    /// <summary>
    /// Creates a new instance of the <see cref="CstNode"/> class.
    /// </summary>
    protected CstNode()
    {
        
    }

    /// <summary>
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract CstNode Accept(CstNodeVisitor visitor);
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

    /// <summary>
    /// Gets the name of the root node.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the children of the root node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Gets a value indicating whether the root node is an epsilon node.
    /// </summary>
    public bool IsEpsilon { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstRoot"/> class.
    /// </summary>
    /// <param name="children"></param>
    public CstRoot(string name, CstNode[] children) 
    {
        Name = name;
        Children = children;
        IsEpsilon = false;
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

    /// <summary>
    /// Gets the name of the internal node.
    /// </summary>
    public string Name { get; }

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
    /// <param name="isEpsilon"></param>
    public CstInternal(string name, CstNode[] children, bool isEpsilon = false) 
    {
        Name = name;
        Children = children;
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

    /// <summary>
    /// Gets the token associated with the leaf node.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstLeaf"/> class.
    /// </summary>
    /// <param name="token"></param>
    public CstLeaf(Token token)
    {
        Token = token;
    }

    ///<inheritdoc/>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitTerminal(this);
    }

    public override string ToString()
    {
        return Token.Value.ToString();
    }

}
