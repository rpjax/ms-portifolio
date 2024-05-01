using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class FirstSetTool
{
    public static FirstSet ComputeFirstSet(ProductionSet set, NonTerminal symbol)
    {
        var derivations = set.Lookup(symbol).ToArray();
        var firsts = new List<Symbol>();

        foreach (var derivation in derivations)
        {
            var sentence = new Sentence(derivation.Body);

            if (sentence.Length == 0)
            {
                throw new InvalidOperationException("Empty sentence in production rule.");
            }

            if (sentence[0] is Terminal terminal)
            {
                firsts.Add(terminal);
                continue;
            }

            if (sentence[0] is Epsilon epsilon)
            {
                firsts.Add(epsilon);
                continue;
            }

            if (sentence[0] is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }

            var target = null as NonTerminal;
            var index = 0;

            while (true)
            {
                target = sentence[index] as NonTerminal;

                if (target is null)
                {
                    throw new InvalidOperationException("The symbol is not a nonterminal.");
                }

                var firstSet = ComputeFirstSet(set, target);

                firsts.AddRange(firstSet.Firsts);

                if (!firstSet.Firsts.Contains(Epsilon.Instance))
                {
                    break;
                }

                firsts.Remove(Epsilon.Instance);

                if (index + 1 >= sentence.Length)
                {
                    break;
                }

                index++;
            }
        }

        return new FirstSet(symbol, firsts.Distinct().ToArray());
    }

    public static FirstSet[] ComputeFirstSets(ProductionSet set)
    {
        var firstSets = new List<FirstSet>();

        foreach (var nonTerminal in set.GetNonTerminals())
        {
            firstSets.Add(ComputeFirstSet(set, nonTerminal));
        }

        return firstSets.ToArray();
    }

}

public class FollowSet
{
    public NonTerminal Symbol { get; }
    public Symbol[] Follows { get; }

    public FollowSet(NonTerminal symbol, Symbol[] follows)
    {
        Symbol = symbol;
        Follows = follows;
    }
}

public class FollowSetTool
{
    public static FirstSet ComputeFollowSet(ProductionSet set, NonTerminal symbol)
    {
        var derivations = set.Lookup(symbol).ToArray();
        var follows = new List<Symbol>();

        foreach (var derivation in derivations)
        {
            var sentence = new Sentence(derivation.Body);

            if (sentence.Length == 0)
            {
                throw new InvalidOperationException("Empty sentence in production rule.");
            }

            if (sentence[0] is Terminal terminal)
            {
                follows.Add(terminal);
                continue;
            }

            if (sentence[0] is Epsilon epsilon)
            {
                follows.Add(epsilon);
                continue;
            }

            if (sentence[0] is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }
        }
    }
}
