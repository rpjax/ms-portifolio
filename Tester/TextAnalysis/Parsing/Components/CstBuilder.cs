using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public class CstBuilder
{
    private List<CstNode> Accumulator { get; }

    public CstBuilder()
    {
        Accumulator = new List<CstNode>();
    }

    public int AccumulatorCount => Accumulator.Count;

    public void AddTerminal(Terminal terminal)
    {
        Accumulator.Add(new TerminalCstNode(terminal));
    }

    public void Reduce(NonTerminal nonTerminal, int count)
    {
        var children = Accumulator
            .Skip(Accumulator.Count - count)
            .ToList();

        Accumulator.RemoveRange(Accumulator.Count - count, count);
        Accumulator.Add(new NonTerminalCstNode(nonTerminal, children));
    }

    public CstNode Build()
    {
        if (Accumulator.Count == 0)
        {
            throw new InvalidOperationException("CST is empty.");
        }
        if (Accumulator.Count != 1)
        {
            throw new InvalidOperationException("CST is not complete.");
        }

        return Accumulator.Single();
    }

}

