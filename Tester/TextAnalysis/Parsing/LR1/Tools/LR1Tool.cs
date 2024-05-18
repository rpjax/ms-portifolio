using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;

public class LR1Tool
{
    public static LR1State[] ComputeStates(
        ProductionSet set)
    {
        set.EnsureNoMacros();
        set.EnsureAugmented();

        var initialState = ComputeInitialState(set);

        var states = new List<LR1State>
        {
            initialState
        };

        while (true)
        {
            var counter = 0;

            foreach (var state in states.ToArray())
            {
                var nextStates = ComputeNextStates(
                    set: set,
                    state: state,
                    kernelBlacklist: Array.Empty<LR1Item>()
                );

                foreach (var nextState in nextStates)
                {
                    if (states.Any(x => x.Equals(nextState)))
                    {
                        continue;
                    }

                    states.Add(nextState);
                    counter++;
                }
            }

            if (counter == 0)
            {
                break;
            }
        }

        return states.ToArray();
    }

    public static Dictionary<int, LR1State> ComputeStateDictionary(
        ProductionSet set)
    {
        var dictionary = new Dictionary<int, LR1State>();
        var states = ComputeStates(set);

        for (int i = 0; i < states.Length; i++)
        {
            dictionary.Add(i, states[i]);
        }

        return dictionary;
    }

    private static LR1State ComputeInitialState(
        ProductionSet set)
    {
        var augmentedProduction = set.TryGetAugmentedStartProduction();

        if (augmentedProduction is null)
        {
            throw new InvalidOperationException("The production set does not have an augmented production.");
        }

        var initialItemProduction = new ProductionRule(
            head: augmentedProduction.Head,
            body: augmentedProduction.Body[0]
        );

        var kernel = new LR1Item(
            production: initialItemProduction,
            position: 0,
            lookaheads: Eoi.Instance
        );

        var closure = ComputeClosure(
            set: set,
            kernel: kernel,
            new()
        );

        return new LR1State(
            kernel: kernel,
            closure: closure
        );
    }

    private static LR1Item[] ComputeClosure(
        ProductionSet set,
        LR1Item kernel,
        List<LR1Item> context)
    {
        var kernelSymbol = kernel.Symbol;
        var kernelSignature = kernel.GetSignature(useLookaheads: true);

        var canExpand = kernelSymbol is not null
            && kernelSymbol.IsNonTerminal
            ;

        if (!canExpand)
        {
            return context.ToArray();
        }

        if (kernelSymbol is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException("The expanded symbol is not a nonterminal.");
        }

        var productions = set.Lookup(nonTerminal)
            .ToArray();

        if (productions.Length == 0)
        {
            throw new InvalidOperationException($"The non-terminal '{nonTerminal}' does not have any productions.");
        }

        foreach (var production in productions)
        {
            var lookaheads = ComputeLookaheads(
                set: set,
                beta: kernel.GetBeta(),
                alpha: kernel.Lookaheads
            );

            var newItem = new LR1Item(
                production: production,
                position: 0,
                lookaheads: lookaheads
            );

            var newItemSignature = newItem.GetSignature(useLookaheads: true);

            var isNewItemClosureProcessed = context
                .Any(item => item.GetSignature(useLookaheads: true) == newItemSignature);

            if (isNewItemClosureProcessed)
            {
                continue;
            }

            context.Add(newItem);

            /*
             * Computes the closure for the new kernel.
             */
            ComputeClosure(
                set: set,
                kernel: newItem,
                context: context
            );

        }

        var groups = context
            .GroupBy(item => item.GetSignature(useLookaheads: false))
            .ToArray();

        var uniqueItems = new List<LR1Item>();

        foreach (var group in groups)
        {
            var production = group.First().Production;
            var position = group.First().Position;
            var lookaheads = group
                .SelectMany(item => item.Lookaheads)
                .Distinct()
                .ToArray();

            var uniqueItem = new LR1Item(
                production: production,
                position: position,
                lookaheads: lookaheads
            );

            uniqueItems.Add(uniqueItem);
        }

        return uniqueItems
            .ToArray();
    }

    private static LR1Item[] ComputeClosure(
        ProductionSet set,
        LR1Item[] kernel)
    {
        var items = new List<LR1Item>();
        var context = new List<LR1Item>();

        foreach (var item in kernel)
        {
            var closure = ComputeClosure(
                set: set,
                kernel: item,
                context: context
            );

            items.AddRange(closure);
        }

        return items
            .Distinct()
            .ToArray();
    }

    private static Terminal[] ComputeLookaheads(
        ProductionSet set,
        Sentence beta,
        Terminal[] alpha)
    {
        var lookaheads = new List<Terminal>();
        var position = 0;

        while (true)
        {
            if (position == beta.Length)
            {
                lookaheads.AddRange(alpha);
                break;
            }

            var symbol = beta[position];

            if (symbol is Epsilon)
            {
                position++;
                continue;
            }

            if (symbol is Terminal terminal)
            {
                lookaheads.Add(terminal);
                break;
            }

            if (symbol is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The lrItemSymbol is not a nonterminal.");
            }

            var productions = set.Lookup(nonTerminal).ToArray();

            if (productions.Length == 0)
            {
                throw new InvalidOperationException($"The non-terminal '{nonTerminal}' does not have any productions.");
            }

            foreach (var production in productions)
            {
                var lookaheadsForProduction = ComputeLookaheads(
                    set: set,
                    beta: production.Body,
                    alpha: alpha
                );

                lookaheads.AddRange(lookaheadsForProduction);
            }

            break;
        }

        return lookaheads
            .Distinct()
            .ToArray();
    }

    private static LR1State[] ComputeNextStates(
        ProductionSet set,
        LR1State state,
        LR1Item[] kernelBlacklist)
    {
        var states = new List<LR1State>();

        var gotosDictionary = ComputeGoto(
            state: state,
            kernelBlacklist: kernelBlacklist
        );

        foreach (var entry in gotosDictionary)
        {
            var nextStateKernel = entry.Value;

            var nextStateClosure = ComputeClosure(
                set: set,
                kernel: nextStateKernel
            );

            var nextState = new LR1State(
                kernel: nextStateKernel,
                closure: nextStateClosure
            );

            states.Add(nextState);
        }

        return states.ToArray();
    }

    private static Dictionary<Symbol, LR1Item[]> ComputeGoto(
        LR1State state,
        LR1Item[] kernelBlacklist)
    {
        var symbolGroups = state.Items
            .Where(item => item.Symbol is not null)
            .GroupBy(item => item.Symbol!)
            .ToArray();

        var dictionary = new Dictionary<Symbol, LR1Item[]>();

        foreach (var entry in symbolGroups)
        {
            var symbol = entry.Key;

            var kernel = entry
                .Select(item => item.GetNextItem())
                .Where(item => !kernelBlacklist.Any(k => k.Equals(item)))
                .ToArray();

            if (kernel.Length == 0)
            {
                continue;
            }

            dictionary.Add(symbol, kernel);
        }

        return dictionary;
    }
}
