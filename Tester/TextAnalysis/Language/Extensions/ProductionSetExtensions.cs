namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionSetExtensions
{
    public static bool ContainsMacro(this ProductionSet set)
    {
        return set.Productions.Any(x => x.ContainsMacro());
    }

    public static void EnsureNoMacros(this ProductionSet set)
    {
        if (ContainsMacro(set))
        {
            throw new InvalidOperationException("The production set contains macros.");
        }
    }

    public static void ExpandMacros(this ProductionSet set)
    {
        while (set.ContainsMacro())
        {
            foreach (var production in set.Productions.ToArray())
            {
                if (production.ContainsMacro())
                {
                    set.Productions.Remove(production);
                    set.Productions.AddRange(production.ExpandMacros());
                }
            }
        }
    }

    public static void ReplaceNonTerminal(this ProductionSet set, NonTerminal current, NonTerminal replacement)
    {
        foreach (var production in set.Productions)
        {
            production.ReplaceNonTerminal(current, replacement);
        }
    }

    /*
        Clean up methods.
    */

    // Tries the following transformations in order: 
    // - remove unreachable nonterminals.
    // - expand unit rules.
    // - remove unrealizable productions.
    public static void AutoClean(this ProductionSet set)
    {
        set.ExpandMacros();
        set.RemoveUnreachableProductions();
        set.ExpandUnitProductions();
        //set.RemoveUnrealizableProductions();
    }

    public static void RemoveUnreachableProductions(this ProductionSet set)
    {
        EnsureNoMacros(set);

        var reachable = new HashSet<NonTerminal>();
        var visited = new HashSet<NonTerminal>();

        var start = set.Start;

        if (start is null)
        {
            throw new InvalidOperationException("The start symbol is not set.");
        }

        reachable.Add(start);

        void Visit(NonTerminal nonTerminal)
        {
            if (visited.Contains(nonTerminal))
            {
                return;
            }

            visited.Add(nonTerminal);

            foreach (var production in set.Lookup(nonTerminal))
            {
                foreach (var symbol in production.Body)
                {
                    if (symbol is NonTerminal nt)
                    {
                        reachable.Add(nt);
                        Visit(nt);
                    }
                }
            }
        }

        Visit(start);

        set.Productions = set.Productions
            .Where(x => reachable.Contains(x.Head))
            .ToList();
    }

    public static int ExpandUnitProductions(this ProductionSet set)
    {
        EnsureNoMacros(set);

        var counter = 0;

        while (set.Any(x => x.Body.Length == 1 && x.Body[0] is NonTerminal))
        {
            var unit = new Dictionary<NonTerminal, NonTerminal>();

            foreach (var production in set.Productions.ToArray())
            {
                if (production.Body.Length == 1 && production.Body[0] is NonTerminal nt)
                {
                    unit[production.Head] = nt;
                    set.Productions.Remove(production);
                    counter++;
                }
            }


        }

        while (set.Any(x => x.Body.Length == 1 && x.Body[0] is NonTerminal))
        {
            var unit = new Dictionary<NonTerminal, NonTerminal>();

            foreach (var production in set.Productions.ToArray())
            {
                if (production.Body.Length == 1 && production.Body[0] is NonTerminal nt)
                {
                    unit[production.Head] = nt;
                    set.Productions.Remove(production);
                    counter++;
                }
            }

            foreach (var production in set.Productions.ToArray())
            {
                for (int i = 0; i < production.Body.Length; i++)
                {
                    if (production.Body[i] is not NonTerminal nonTerminal)
                    {
                        continue;
                    }

                    if (unit.TryGetValue(nonTerminal, out var nt))
                    {
                        var body = new List<ProductionSymbol>()
                            .FluentAdd(production.Body.Take(i).ToArray())
                            .FluentAdd(nt)
                            .FluentAdd(production.Body.Skip(i + 1).ToArray())
                            .ToArray();

                        set.AddProduction(production.Head, body);
                    }
                }
            }
        }

        return counter;
    }

    // A nonterminal A is unrealizable iff there is no derivation A ⇒* x1 ... xn where x1 ... xn is a (possibly empty) string of terminals.
    public static void RemoveUnrealizableProductions(this ProductionSet set)
    {
        EnsureNoMacros(set);

        var realizable = new HashSet<NonTerminal>();
        var visited = new HashSet<NonTerminal>();

        var start = set.Start;

        if (start is null)
        {
            throw new InvalidOperationException("The start symbol is not set.");
        }

        realizable.Add(start);

        void Visit(NonTerminal nonTerminal)
        {
            if (visited.Contains(nonTerminal))
            {
                return;
            }

            visited.Add(nonTerminal);

            foreach (var production in set.Lookup(nonTerminal))
            {
                if (production.Body.All(x => x is Terminal || realizable.Contains(x.AsNonTerminal())))
                {
                    realizable.Add(nonTerminal);
                }
                else
                {
                    foreach (var symbol in production.Body)
                    {
                        if (symbol is NonTerminal nt)
                        {
                            Visit(nt);
                        }
                    }
                }
            }
        }

        Visit(start);

        if (realizable.Count != set.GetNonTerminals().Count())
        {
            throw new InvalidOperationException("The grammar is not realizable.");
        }

        set.Productions = set.Productions
            .Where(x => realizable.Contains(x.Head))
            .ToList();
    }

    // public static void RemoveUnproductiveProductions(this ProductionSet set)
    // {
    //     var productive = new HashSet<NonTerminal>();
    //     var visited = new HashSet<NonTerminal>();

    //     var start = set.Start;

    //     if (start is null)
    //     {
    //         throw new InvalidOperationException("The start symbol is not set.");
    //     }

    //     productive.Add(start);

    //     void Visit(NonTerminal nonTerminal)
    //     {
    //         if (visited.Contains(nonTerminal))
    //         {
    //             return;
    //         }

    //         visited.Add(nonTerminal);

    //         foreach (var production in set.Lookup(nonTerminal))
    //         {
    //             if (production.Body.All(x => x is Terminal || productive.Contains(x as NonTerminal)))
    //             {
    //                 productive.Add(nonTerminal);
    //             }
    //             else
    //             {
    //                 foreach (var symbol in production.Body)
    //                 {
    //                     if (symbol is NonTerminal nt)
    //                     {
    //                         Visit(nt);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     Visit(start);

    //     set.Productions = set.Productions
    //         .Where(x => productive.Contains(x.Head))
    //         .ToList();
    // }

    public static int LeftFactor(this ProductionSet set)
    {
        EnsureNoMacros(set);

        int counter = 0;

        foreach (var nonTerminal in set.GetNonTerminals().ToArray())
        {
            var productions = set.Lookup(nonTerminal)
                .ToArray();

            if (productions.All(x => !x.IsLeftRecursive()))
            {
                continue;
            }

            var recursiveProductions = productions
                .Where(productions => productions.IsLeftRecursive())
                .ToArray();

            var alphas = productions
                .Where(productions => productions.IsLeftRecursive())
                .Select(x => x.Body.Skip(1).ToArray())
                .ToArray();

            var betas = productions
                .Where(productions => !productions.IsLeftRecursive())
                .Select(x => x.Body.ToArray())
                .ToArray();

            set.RemoveProductions(productions);

            var newNonTerminal = new NonTerminal(nonTerminal.Name + "′");

            foreach (var beta in betas)
            {
                var body = new List<ProductionSymbol>()
                    .FluentAdd(beta)
                    .FluentAdd(newNonTerminal)
                    .ToArray();

                /*
                 * if:
                 * A -> Aa | ε
                 * 
                 * then:
                 * A -> A'
                 * 
                 * instead of:
                 * A -> εA'
                 * 
                 * It removes β from the production if it's value is ε
                 */
                if (body.Length == 2 && body[0] is Epsilon)
                {
                    body = new ProductionSymbol[] { newNonTerminal };
                }

                set.AddProduction(nonTerminal, body);
            }

            foreach (var alpha in alphas)
            {
                var body = new List<ProductionSymbol>()
                    .FluentAdd(alpha)
                    .FluentAdd(newNonTerminal)
                    .ToArray();

                set.AddProduction(newNonTerminal, body);
            }

            set.AddProduction(newNonTerminal, new Epsilon());
            counter++;
        }

        return counter;
    }

    public static void RemoveDirectLeftRecursion(this ProductionSet set)
    {
        while (set.Productions.Any(x => x.IsLeftRecursive()))
        {
            set.LeftFactor();
        }
    }


}
