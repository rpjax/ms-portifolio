using System.Collections;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FollowSet : IEnumerable<Symbol>
{
    public NonTerminal Symbol { get; }
    public Symbol[] Follows { get; }

    public FollowSet(NonTerminal symbol, Symbol[] follows)
    {
        Symbol = symbol;
        Follows = follows;
    }

    public int Length => Follows.Length;

    public override string ToString()
    {
        var follows = string.Join(", ", Follows.Select(x => x.ToString()));
        return $"Follow({Symbol}): {{ {follows} }}";
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Follows).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Follows.GetEnumerator();
    }
}

public class FollowTable
{
    private Dictionary<NonTerminal, FollowSet> Table { get; }

    public FollowTable(FollowSet[] sets)
    {
        Table = sets.ToDictionary(x => x.Symbol);
    }

    public FollowSet Lookup(NonTerminal symbol)
    {
        if (!Table.ContainsKey(symbol))
        {
            throw new InvalidOperationException("The symbol is not in the table.");
        }

        return Table[symbol];
    }

    public override string ToString()
    {
        return string.Join("\n", Table.Values.Select(x => x.ToString()));
    }

}

