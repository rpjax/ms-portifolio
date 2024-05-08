namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class ParsingTable
{
    private Dictionary<(NonTerminal, Terminal), ProductionRule> Entries = new();

    public ParsingTable(Dictionary<(NonTerminal, Terminal), ProductionRule> entries)
    {
        Entries = entries;
    }

    public override string ToString()
    {
        var keys = Entries.Keys
            .Select(x => $"{x.Item1} + {x.Item2}")
            ;
        var values = Entries.Values
            .Select(x => x.ToString())
            ;

        return string.Join("\n", keys.Zip(values, (k, v) => $"[{k}] = ({v})"));
    }

    public ProductionRule? Lookup(NonTerminal nonTerminal, Terminal terminal)
    {
        if (Entries.TryGetValue((nonTerminal, terminal), out var production))
        {
            return production;
        }

        return null;
    }

}
