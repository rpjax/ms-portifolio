namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents a prodution rule, ex: &lt;expression&gt; ::= &lt;expression-lhs&gt; ':' &lt;expression-rhs&gt; <br/><br/>
/// This exists for debug purposes. It has no functional meaning, the productions and the parsing is done manually, <br/>
/// with no state machine. This may change in the future, with the introduction of proper grammar interpretation, using BNF format.
/// </summary>
public class SymbolProduction
{
    public string Name { get; }
    public List<SymbolTypeCollection> FormationRule { get; }
    
    public SymbolProduction(string name, List<SymbolTypeCollection> rule)
    {
        Name = name;
        FormationRule = rule;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

public struct SymbolTypeCollection
{
    public NodeType[] Types { get; }

    public SymbolTypeCollection(NodeType[] types)
    {
        Types = types;
    }
}
