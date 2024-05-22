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
                var kernelBlacklist = states
                    .SelectMany(x => x.Kernel)
                    .ToArray();

                var nextStates = ComputeNextStates(
                    set: set,
                    state: state,
                    kernelBlacklist: kernelBlacklist
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

    public static Dictionary<int, LR1State> ComputeStatesDictionary(
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
        var augmentedProduction = set.GetAugmentedStartProduction();

        var initialItemProduction = new ProductionRule(
            head: augmentedProduction.Head,
            body: augmentedProduction.Body[0]
        );

        var kernel = new LR1Item(
            production: initialItemProduction,
            position: 0,
            lookaheads: Eoi.Instance
        );

        var closure = ComputeKernelClosure(
            set: set,
            kernel: new LR1Item[] { kernel }
        );

        return new LR1State(
            kernel: kernel,
            closure: closure
        );
    }

    private static LR1State[] ComputeNextStates(
    ProductionSet set,
    LR1State state,
    LR1Item[] kernelBlacklist)
    {
        var states = new List<LR1State>();

        var gotosDictionary = ComputeGotoDictionary(
            state: state,
            kernelBlacklist: kernelBlacklist
        );

        foreach (var entry in gotosDictionary)
        {
            var nextStateKernel = entry.Value;

            var nextStateClosure = ComputeKernelClosure(
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

    /*
     * Closure rewritten to use a stack instead of recursion.
     */
    private static LR1Item[] ComputeItemClosure(
        ProductionSet set,
        LR1Item item)
    {
        var items = new List<LR1Item>();

        var symbol = item.Symbol;

        if (symbol is not NonTerminal nonTerminal)
        {
            return items.ToArray();
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
                beta: item.GetBeta(),
                originalLookaheads: item.Lookaheads
            );

            var newItem = new LR1Item(
                production: production,
                position: 0,
                lookaheads: lookaheads
            );

            items.Add(newItem);
        }

        return items.ToArray();
    }

    private static LR1Item[] ComputeKernelClosure(
        ProductionSet set,
        LR1Item[] kernel)
    {
        var items = new List<LR1Item>(kernel);

        while (true)
        {
            var counter = 0;

            foreach (var item in items.ToArray())
            {
                var closure = ComputeItemClosure(
                    set: set,
                    item: item
                );

                foreach (var newItem in closure)
                {
                    if (items.Any(x => x == newItem))
                    {
                        continue;
                    }

                    items.Add(newItem);
                    counter++;
                }
            }

            if (counter == 0)
            {
                break;
            }
        }

        //return items
        //    .Skip(kernel.Length)
        //    .ToArray();

        /*
         * This section is commented out because i'm currently figuring out if the code bellow is correct for LR(1). 
         * It combines kernels, withing the same closure, with identical productions and dot positions but different lookaheads. 
         * Ex: (A -> .a b, {c}) and (A -> .a b, {d}) into (A -> .a b, {c, d}).
         * 
         * NOTE: for now i'll uncomment it because it seems to be working.
         */

        var groups = items
            .Skip(kernel.Length)
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

        /*
         * some debugging code
         */
        //return items
        //    .Skip(kernel.Length)
        //    .Select(x => new { Item = x, Signature = x.GetSignature(useLookaheads:true) })
        //    .DistinctBy(x => x.Signature)
        //    .Select(x => x.Item)
        //    .ToArray();
    }

    private static Terminal[] ComputeLookaheads(
        ProductionSet set,
        Sentence beta,
        Terminal[] originalLookaheads)
    {
        var stack = new Stack<Sentence>();
        var lookaheads = new List<Terminal>();

        stack.Push(beta);

        while (true)
        {
            if (stack.Count == 0)
            {
                break;
            }

            var sentence = stack.Pop();

            if (sentence.Length == 0)
            {
                lookaheads.AddRange(originalLookaheads);
                continue;
            }

            var symbol = sentence[0];

            if (symbol is Terminal terminal)
            {
                lookaheads.Add(terminal);
                continue;
            }

            if (symbol is Epsilon)
            {
                throw new NotImplementedException();
            }

            if (symbol is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }

            var productions = set.Lookup(nonTerminal).ToArray();
            var producesEpsilon = productions.Any(x => x.IsEpsilonProduction);

            // S -> .A B c (beta is B c)
            // B -> b
            // B -> .epsilon

            foreach (var production in productions)
            {
                if (production.IsEpsilonProduction)
                {
                    stack.Push(sentence.Skip(1).ToArray());
                    continue;
                }

                var newSentence = new Sentence(
                    production.Body.Concat(sentence.Skip(1)).ToArray()
                );

                if (newSentence.Length == 0)
                {
                    Console.WriteLine();
                }

                stack.Push(newSentence);
            }
        }

        return lookaheads
            .Distinct()
            .ToArray();
    }

    private static Dictionary<Symbol, LR1Item[]> ComputeGotoDictionary(
        LR1State state,
        LR1Item[] kernelBlacklist)
    {
        var symbolGroups = state.Items
            .Where(item => item.Symbol is not null)
            .Where(item => item.Symbol is not Epsilon)
            .GroupBy(item => item.Symbol!)
            .ToArray();

        var dictionary = new Dictionary<Symbol, LR1Item[]>();

        foreach (var entry in symbolGroups)
        {
            var symbol = entry.Key;

            var kernel = entry
                .Select(item => item.GetNextItem())
                .Where(item => !kernelBlacklist.Any(blacklistedKernel => blacklistedKernel.ContainsItem(item)))
                //.Where(item => !kernelBlacklist.Any(blacklistedKernel => blacklistedKernel.Equals(item)))
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
