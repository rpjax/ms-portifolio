using ModularSystem.Core.TextAnalysis.Language.Components;

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
    /// Gets the symbol associated with the node.
    /// </summary>
    public Symbol Symbol { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstNode"/> class.
    /// </summary>
    /// <param name="symbol"></param>
    protected CstNode(Symbol symbol)
    {
        Symbol = symbol;
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
    public override CstNodeType Type => CstNodeType.Root;
    public CstNode[] Children { get; }

    public CstRoot(Symbol symbol, CstNode[] children) : base(symbol)
    {
        Children = children;
    }

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
    public override CstNodeType Type => CstNodeType.Internal;

    public CstNode[] Children { get; }

    public CstInternal(NonTerminal symbol, CstNode[] children) : base(symbol)
    {
        Children = children;
    }

    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitNonTerminal(this);
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }
}

/// <summary>
/// Represents a leaf node in the concrete syntax tree (CST).
/// </summary>
public class CstLeaf : CstNode
{
    public override CstNodeType Type => CstNodeType.Leaf;
    public bool IsEpsilon { get; }

    public CstLeaf(Symbol symbol, bool isEpsilon = false) : base(symbol)
    {
        IsEpsilon = isEpsilon;
    }

    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitTerminal(this);
    }

    public override string ToString()
    {
        if(IsEpsilon)
        {
            return $"ε ({Symbol.ToString()})";
        }

        return Symbol.ToString();
    }

}
