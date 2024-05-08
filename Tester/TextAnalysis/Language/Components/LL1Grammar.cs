using ModularSystem.Core.TextAnalysis.Language.Tools;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class LL1Grammar : Grammar
{
    private ParsingTable? ParsingTable { get; set; }

    public LL1Grammar(NonTerminal start, IEnumerable<ProductionRule> productions) : base(start, productions)
    {
    }

    public LL1Grammar(ProductionSet set) : base(set)
    {
    }

    public void ComputeParsingTable()
    {
        ParsingTable = ParsingTableTool.ComputeParsingTable(Productions);
    }

    public ParsingTable GetParsingTable()
    {
        if (ParsingTable == null)
        {
            ParsingTable = ParsingTableTool.ComputeParsingTable(Productions);
        }

        return ParsingTable;
    }

}

