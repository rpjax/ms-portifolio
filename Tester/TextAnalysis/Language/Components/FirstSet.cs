using System.Collections;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FirstSet : IEnumerable<Symbol>
{
    public NonTerminal Symbol { get; }
    public Symbol[] Firsts { get; }

    public FirstSet(NonTerminal symbol, Symbol[] firsts)
    {
        Symbol = symbol;
        Firsts = firsts;
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Firsts).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Firsts).GetEnumerator();
    }

    public override string ToString()
    {
        var firsts = string.Join(", ", Firsts.Select(x => x.ToString()));
        return $"First({Symbol}): {{ {firsts} }}";
    }

    public bool Overlaps(FirstSet other)
    {
        return Firsts.Any(x => x != Epsilon.Instance && other.Firsts.Contains(x));
    }

    public bool Overlaps(FollowSet followSet)
    {
        return Firsts.Any(x => x != Epsilon.Instance && followSet.Contains(x));
    }

}
