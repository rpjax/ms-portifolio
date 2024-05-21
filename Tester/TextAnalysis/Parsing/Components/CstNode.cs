using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public enum CstNodeType
{
    Terminal,
    NonTerminal,
    Epsilon
}

public abstract class CstNode
{
    public abstract CstNodeType NodeType { get; }
}

public class TerminalCstNode : CstNode
{
    public override CstNodeType NodeType => CstNodeType.Terminal;
    public Terminal Terminal { get; }

    public TerminalCstNode(Terminal terminal)
    {
        Terminal = terminal;
    }

    public override string ToString()
    {
        return Terminal.ToString();
    }
}

public class NonTerminalCstNode : CstNode
{
    public override CstNodeType NodeType => CstNodeType.NonTerminal;
    public NonTerminal NonTerminal { get; }
    public List<CstNode> Children { get; }

    public NonTerminalCstNode(NonTerminal nonTerminal, List<CstNode> children)
    {
        NonTerminal = nonTerminal;
        Children = children;
    }

    public override string ToString()
    {
        return NonTerminal.ToString();
    }
}

class EpsilonCstNode : CstNode
{
    public override CstNodeType NodeType => CstNodeType.Epsilon;

    public override string ToString() => "ε";
}
