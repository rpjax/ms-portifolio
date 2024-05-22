using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public class CstBuilder
{
    private List<CstNode> Accumulator { get; }
    private bool UseEpsilons { get; set; }

    public CstBuilder(bool useEpsilons = false)
    {
        Accumulator = new List<CstNode>();
        UseEpsilons = useEpsilons;
    }

    public int AccumulatorCount => Accumulator.Count;

    public void AddTerminal(Terminal terminal)
    {
        Accumulator.Add(new CstTerminal(terminal));
    }

    public void ReduceEpsilon(NonTerminal nonTerminal)
    {
        Accumulator.Add(new CstEpsilon(nonTerminal));
    }

    public void Reduce(NonTerminal nonTerminal, int length)
    {
        var offset = Accumulator.Count - length;
        var children = Accumulator
            .Skip(offset)
            .ToList();

        Accumulator.RemoveRange(offset, length);

        if(UseEpsilons)
        {
            children = children
                .ToList();
        }
        else
        {
            children = children
                .Where(c => c is not CstEpsilon)
                .ToList();
        }

        Accumulator.Add(new CstNonTerminal(nonTerminal, children.ToArray()));
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

