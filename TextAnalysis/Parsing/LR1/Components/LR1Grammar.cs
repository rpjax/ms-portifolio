using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1Grammar : Grammar
{
    public LR1Grammar(NonTerminal start, IEnumerable<ProductionRule> productions) : base(start, productions)
    {
    }
}
