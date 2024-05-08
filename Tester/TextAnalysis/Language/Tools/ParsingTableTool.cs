using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class ParsingTableTool
{
    public static ParsingTable ComputeParsingTable(ProductionSet set)
    {
        var entries = new Dictionary<(NonTerminal, Terminal), ProductionRule>();

        foreach (var production in set)
        {
            var firstSet = production.ComputeFirstSet(set);
            var containsEpsilon = firstSet.Contains(Epsilon.Instance);

            foreach (var symbol in firstSet)
            {
                if(symbol is Epsilon)
                {
                    continue;
                }
                if (symbol is not Terminal terminal)
                {
                    throw new InvalidOperationException("Invalid symbol in first set");
                }

                entries[(production.Head, terminal)] = production;
            }

            if (containsEpsilon)
            {
                var followSet = production.ComputeFollowSet(set);

                foreach (var symbol in followSet)
                {
                    if(symbol is not Terminal terminal)
                    {
                        throw new InvalidOperationException("Invalid symbol in follow set");
                    }

                    entries[(production.Head, terminal)] = production;
                }

                continue;
            }

        }

        return new ParsingTable(entries);
    }
}
