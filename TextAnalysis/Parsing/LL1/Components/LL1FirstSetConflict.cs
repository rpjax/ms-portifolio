using ModularSystem.TextAnalysis.Language.Components;

namespace ModularSystem.TextAnalysis.Parsing.LL1.Components;

public class LL1FirstSetConflict
{
    public NonTerminal NonTerminal { get; }
    public Symbol[] ClashingSymbols { get; }

    public LL1FirstSetConflict(NonTerminal nonTerminal, Symbol[] clashingSymbols)
    {
        NonTerminal = nonTerminal;
        ClashingSymbols = clashingSymbols;
    }
}
