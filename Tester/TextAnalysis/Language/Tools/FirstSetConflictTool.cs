using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FirstSetConflict
{
    public NonTerminal NonTerminal { get; }
    public Symbol[] ClashingSymbols { get; }

    public FirstSetConflict(NonTerminal nonTerminal, Symbol[] clashingSymbols)
    {
        NonTerminal = nonTerminal;
        ClashingSymbols = clashingSymbols;
    }
}

public class FirstSetConflictTool
{
    public static FirstSetConflict[] ComputeFirstSetConflicts(ProductionSet set)
    {
        var firstTable = set.ComputeFirstTable();

        var clashes = new List<FirstSetConflict>();

        foreach (var production in set)
        {
            var firstSet = production.ComputeFirstSet(set);
            var firsts = firstSet.Firsts;

            foreach (var symbol in firsts)
            {
                if (symbol is Terminal terminal)
                {
                    var clashingSymbols = firsts
                        .GroupBy(x => x)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .ToArray();

                    if (clashingSymbols.Length != 0)
                    {
                        clashes.Add(new FirstSetConflict(production.Head, clashingSymbols));
                    }
                }
            }
        }

        return clashes.ToArray();
    }
}
