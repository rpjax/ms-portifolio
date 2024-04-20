using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language;

/*
    TODO: take a look at the tarjan's algorithm for finding cycles in a graph.
*/

/// <summary>
/// The purpose of this class is to provide a tool for finding indirect left recursion.
/// </summary>
public static class RecursionTool
{
    public static IEnumerable<LeftRecursionCicle> GetLeftRecursionCicles(GrammarDefinition grammar)
    {
        foreach (var nonTerminal in grammar.GetNonTerminals())
        {
            foreach (var cicle in GetLeftRecursionCicles(grammar.Productions, nonTerminal))
            {
                yield return cicle;
            }
        }
    }

    public static IEnumerable<LeftRecursionCicle> GetLeftRecursionCicles(
        ProductionSet productions,
        NonTerminal test)
    {
        return GetLeftRecursionCicles(
            sentence: new Sentence(test),
            productions: productions,
            test: test,
            stack: new List<Derivation>(),
            isFirstIteration: true
       );
    }

    public static IEnumerable<LeftRecursionCicle> GetLeftRecursionCicles(
        Sentence sentence,
        ProductionSet productions,
        NonTerminal test,
        List<Derivation> stack,
        bool isFirstIteration)
    {
        var leftmostSymbol = sentence.GetLeftmostSymbol();

        if (leftmostSymbol.IsTerminal)
        {
            yield break;
        }

        var nonTerminal = leftmostSymbol.AsNonTerminal();

        if (!isFirstIteration && nonTerminal == test)
        {
            yield return new LeftRecursionCicle(stack);
        }

        var nonTerminalIsInStack = stack
            .Any(x => x.OriginalSentence.GetLeftmostSymbol() == nonTerminal);

        if (nonTerminalIsInStack)
        {
            yield break;

            var nonTerminalIndex = stack
                .FindIndex(x => x.OriginalSentence.GetLeftmostSymbol() == nonTerminal);

            if (nonTerminalIndex < 0)
            {
                throw new InvalidOperationException("The non-terminal is not in the stack.");
            }

            var cicleStack = stack.Select((x, i) => new { x, i })
                .Where(x => x.i >= nonTerminalIndex)
                .Select(x => x.x)
                .ToList();

            yield return new LeftRecursionCicle(cicleStack);

        }

        var nonTerminalProductions = productions
            .Lookup(nonTerminal);

        foreach (var production in nonTerminalProductions)
        {
            var derivation = sentence.DeriveLeftmostNonTerminal(production);
            var newStack = new List<Derivation>(stack) { derivation };

            foreach (var cicle in GetLeftRecursionCicles(derivation.DerivedSentence, productions, test, newStack, false))
            {
                yield return cicle;
            }
        }

        yield break;
    }

}

