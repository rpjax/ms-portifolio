using ModularSystem.Core;
using ModularSystem.Core.Extensions;
using ModularSystem.TextAnalysis.Language.Components;
using ModularSystem.TextAnalysis.Language.Extensions;

namespace ModularSystem.TextAnalysis.Parsing.LL1.Components;

public class LL1LeftRecursionCicle
{
    public Derivation[] Derivations { get; }

    public LL1LeftRecursionCicle(IEnumerable<Derivation> derivations)
    {
        Derivations = derivations.ToArray();

        if (Derivations.Length == 0)
        {
            throw new ArgumentException("The derivations array must have at least one element.");
        }
        if (derivations.Any(x => x.OriginalSentence.IsEmpty() || x.DerivedSentence.IsEmpty()))
        {
            throw new ArgumentException("The derivations must not have empty sentences.");
        }

        var start = derivations.First().OriginalSentence.GetLeftmostNonTerminal();

        if (start is null)
        {
            throw new ArgumentException("The first derivation must have a leftmost non-terminal.");
        }

        var lastNonTerminal = derivations.Last().DerivedSentence.GetLeftmostNonTerminal();

        if (lastNonTerminal is null)
        {
            throw new ArgumentException("The last derivation must have a leftmost non-terminal.");
        }

        if (lastNonTerminal != start)
        {
            throw new InvalidOperationException("The leftmost symbol of the last derivation must be the same as the start non-terminal.");
        }
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Derivations.Select(x => x.ToString()));
    }
}
