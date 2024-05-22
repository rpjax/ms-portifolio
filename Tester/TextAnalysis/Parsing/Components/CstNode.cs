using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the type of a node in the concrete syntax tree (CST).
/// </summary>
public enum CstNodeType
{
    Terminal,
    NonTerminal,
    Epsilon
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
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract CstNode Accept(CstNodeVisitor visitor);
}

/// <summary>
/// Represents a symbol node in the concrete syntax tree (CST).
/// </summary>
public class CstTerminal : CstNode
{
    public override CstNodeType Type => CstNodeType.Terminal;
    public Terminal Symbol { get; }

    public CstTerminal(Terminal symbol)
    {
        Symbol = symbol;
    }

    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitTerminal(this);
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }

}

/// <summary>
/// Represents a non-symbol node in the concrete syntax tree (CST).
/// </summary>
public class CstNonTerminal : CstNode
{
    public override CstNodeType Type => CstNodeType.NonTerminal;
    public NonTerminal Symbol { get; }
    public CstNode[] Children { get; }

    public CstNonTerminal(NonTerminal symbol, CstNode[] children)
    {
        Symbol = symbol;
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
/// Represents an epsilon node in the concrete syntax tree (CST).
/// </summary>
public class CstEpsilon : CstNode
{
    public override CstNodeType Type => CstNodeType.Epsilon;
    public NonTerminal NonTerminal { get; }

    public CstEpsilon(NonTerminal nonTerminal)
    {
        NonTerminal = nonTerminal;
    }

    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitEpsilon(this);
    }

    public override string ToString() => "ε";
}
