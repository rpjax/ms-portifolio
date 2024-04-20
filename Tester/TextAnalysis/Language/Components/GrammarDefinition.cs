namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public class GrammarDefinition
{
    public ProductionSet Productions { get; private set; }
    public NonTerminal Start { get; }

    public GrammarDefinition(ProductionRule[] productions, NonTerminal? start = null)
    {
        Productions = productions;
        Start = start ?? productions.First().Head;

        if (Start is null)
        {
            throw new ArgumentException("The start symbol cannot be null.");
        }
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

    public override string ToString()
    {
        var productionGroups = Productions
            .GroupBy(x => x.Head.Name);

        var productionsStr = productionGroups
            .Select(x => string.Join(Environment.NewLine, x.Select(y => y.ToString())))
            .ToArray()
            ;

        return string.Join(Environment.NewLine, productionsStr);
    }

}
