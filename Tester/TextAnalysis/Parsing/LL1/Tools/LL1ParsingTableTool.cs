using ModularSystem.Core.TextAnalysis.Language;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LL1.Tools;

public class LL1ParsingTableTool
{
    public static LL1ParsingTable ComputeLL1ParsingTable(ProductionSet set)
    {
        var entries = new List<LL1ParsingTableEntry>();

        foreach (var production in set)
        {
            var firstSet = production.ComputeFirstSet(set);
            var containsEpsilon = firstSet.Contains(Epsilon.Instance);

            foreach (var symbol in firstSet)
            {
                if (symbol is Epsilon)
                {
                    continue;
                }
                if (symbol is not Terminal terminal)
                {
                    throw new InvalidOperationException("Invalid symbol in first set");
                }

                entries.Add(new LL1ParsingTableEntry(production.Head, terminal, production));
            }

            if (containsEpsilon)
            {
                var followSet = production.ComputeFollowSet(set);

                foreach (var symbol in followSet)
                {
                    if (symbol is not Terminal terminal)
                    {
                        throw new InvalidOperationException("Invalid symbol in follow set");
                    }

                    entries.Add(new LL1ParsingTableEntry(production.Head, terminal, production));
                }

                continue;
            }

        }

        return new LL1ParsingTable(entries);
    }
}
