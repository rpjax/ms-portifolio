using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FirstTable
{
    private Dictionary<NonTerminal, FirstSet> Entries { get; }

    public FirstTable(FirstSet[] sets)
    {
        Entries = sets.ToDictionary(x => x.Symbol);
    }

    public FirstSet Lookup(NonTerminal symbol)
    {
        if (!Entries.ContainsKey(symbol))
        {
            throw new InvalidOperationException("The symbol is not in the table.");
        }

        return Entries[symbol];
    }

    public override string ToString()
    {
        return string.Join("\n", Entries.Values.Select(x => x.ToString()));
    }

}
