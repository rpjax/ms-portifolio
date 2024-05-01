using System.Text;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FirstSet
{
    public NonTerminal Symbol { get; }
    public Symbol[] Firsts { get; }

    public FirstSet(NonTerminal symbol, Symbol[] firsts)
    {
        Symbol = symbol;
        Firsts = firsts;
    }

    public override string ToString()
    {
        var firsts = string.Join(", ", Firsts.Select(x => x.ToString()));
        return $"First({Symbol}): {{ {firsts} }}";
    }
}
