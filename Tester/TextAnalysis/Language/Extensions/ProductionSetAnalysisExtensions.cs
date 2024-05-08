using ModularSystem.Core.TextAnalysis.Language.Tools;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionSetAnalysisExtensions
{
    /*
        Analysis helpers.    
    */
    public static bool ContainsMacro(this ProductionSet set)
    {
        return set.Any(x => x.ContainsMacro());
    }

    public static bool ContainsUnitProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return set.Any(x => x.IsUnitProduction());
    }

    public static bool ContainsUnreachableProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return set.GetUnreachableProductions().Length != 0;
    }

    public static bool ContainsUnrealizableProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolRealizabilityTool()
            .Execute(set).Length != 0;
    }

    public static IEnumerable<IGrouping<NonTerminal, ProductionRule>> GroupByHead(this ProductionSet set)
    {
        return set.GroupBy(x => x.Head);
    }

    public static Symbol[] GetUnreachableSymbols(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolReachabilityTool()
            .Execute(set);
    }

    public static ProductionSet GetUnreachableProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var reachableSymbols = set.GetUnreachableSymbols();

        var unreachableProductions = set
            .Where(x => !reachableSymbols.Contains(x.Head))
            .ToArray();

        return new ProductionSet(set.Start, unreachableProductions);
    }

    public static NonTerminal[] GetUnrealizableNonTerminals(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolRealizabilityTool()
            .Execute(set);
    }

    public static ProductionSet GetUnrealizableProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var unrealizableNonTerminals = set.GetUnrealizableNonTerminals();
        var productions = new List<ProductionRule>();

        foreach (var nonTerminal in unrealizableNonTerminals)
        {
            productions.AddRange(set.Lookup(nonTerminal));
        }

        return new ProductionSet(set.Start, productions);
    }

    public static LeftRecursionCicle[] GetLeftRecursionCicles(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var recursiveBranches = new LeftRecursionTool()
            .Execute(set);

        var cicles = new List<LeftRecursionCicle>();

        foreach (var branch in recursiveBranches)
        {
            var derivations = new List<Derivation>();

            foreach (var node in branch.Nodes)
            {
                if (node.Production is null)
                {
                    continue;
                }
                if (node.Symbol is not NonTerminal nonTerminal)
                {
                    throw new InvalidOperationException("The symbol is not a nonterminal.");
                }

                var originalSentence = new Sentence();

                if (node.Parent?.Production is not null)
                {
                    originalSentence = originalSentence.Add(node.Parent.Production.Body);
                }

                var derivedSentence = new Sentence(node.Production.Body);

                var derivation = new Derivation(
                    production: node.Production,
                    nonTerminal: nonTerminal,
                    originalSentence: originalSentence,
                    derivedSentence: derivedSentence
                );

                derivations.Add(derivation);
            }

            cicles.Add(new LeftRecursionCicle(derivations));
        }

        return cicles.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByCommonTerminalLeftFactor(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductions = group
                .Where(x => x.Body.Length > 0)
                .Where(x => x.Body[0].IsTerminal)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductions.Length == 0)
            {
                continue;
            }

            subsets.Add(set.CreateSubset(commonPrefixProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByCommonNonTerminalLeftFactor(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductions = group
                .Where(x => x.Body.Length > 0)
                .Where(x => x.Body[0].IsNonTerminal)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductions.Length == 0)
            {
                continue;
            }

            subsets.Add(set.CreateSubset(commonPrefixProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByFirstSetConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var productionsGroup = set
            .GroupByHead()
            .ToArray();

        for (int i = 0; i < productionsGroup.Length; i++)
        {
            var overlappingProductions = new List<ProductionRule>();
            var group = productionsGroup[i];

            var firstSets = group
                .Select(x => x.ComputeFirstSet(set))
                .ToArray();

            var overlappingSets = firstSets
                .Where(firstSet1 => firstSets.Any(firstSet2 => !ReferenceEquals(firstSet1, firstSet2) && firstSet1.Overlaps(firstSet2)))
                .ToArray();

            if (overlappingSets.Length < 2)
            {
                continue;
            }

            var indexes = overlappingSets
                .Select(x => Array.IndexOf(firstSets, x))
                .ToArray();

            var _overlappingProductions = group
                .Where((_, index) => indexes.Contains(index))
                .ToArray();

            foreach (var production in _overlappingProductions)
            {
                if (!overlappingProductions.Contains(production))
                {
                    overlappingProductions.Add(production);
                }
            }

            subsets.Add(set.CreateSubset(_overlappingProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByFirstFollowConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        foreach (var production in set)
        {
            if (production.Body.Length < 1)
            {
                continue;
            }

            if(production.Body[0] is not NonTerminal nonTerminal)
            {
                continue;
            }

            if(set.Lookup(nonTerminal).All(x => !x.IsEpsilonProduction()))
            {
                continue;
            }
 
            var firstSet = FirstSetTool.ComputeFirstSet(set, nonTerminal);
            var followSet = FollowSetTool.ComputeFollowSet(set, nonTerminal);

            if (firstSet.Overlaps(followSet))
            {
                subsets.Add(set.CreateSubset(set.Lookup(nonTerminal)));
            }
        }

        return subsets.ToArray();
    }

    public static NonTerminal[] GetCommonPrefixProductionHeads(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        var heads = new List<NonTerminal>();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductionsGroup = group
                .Where(x => x.Body.Length > 1)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductionsGroup.Length == 0)
            {
                continue;
            }

            /*
                This part gets the full set of productionsGroup for the given non-terminal.
                The alternative productionsGroup must be preserved.
            */

            var head = commonPrefixProductionsGroup.First().Head;

            if (!heads.Contains(head))
            {
                heads.Add(head);
            }
        }

        return heads.ToArray();
    }

    public static FirstTable ComputeFirstTable(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return FirstSetTool.ComputeFirstTable(set);
    }

    public static FollowTable ComputeFollowTable(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return FollowSetTool.ComputeFollowTable(set);
    }

    public static FirstSetConflict[] ComputeFirstSetConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return FirstSetConflictTool.ComputeFirstSetConflicts(set);
    }

    /*
        Assertion helpers.
    */
    public static void EnsureNoMacros(this ProductionSet set)
    {
        if (set.ContainsMacro())
        {
            throw new InvalidOperationException("The production set contains macros.");
        }
    }

    public static Error[] GetErrors(this ProductionSet set)
    {
        var errors = new List<Error>();

        var unrealizableProductions = set.GetUnrealizableProductions();

        if (unrealizableProductions.Length != 0)
        {
            var error = new Error("The grammar contains unrealizable productionsGroup.")
                .AddDetails("The unrealizable productionsGroup", unrealizableProductions.ToString())
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The unrealizable productionsGroup", unrealizableProductions)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        var unreachableProductions = set.GetUnreachableProductions();

        if (unreachableProductions.Length != 0)
        {
            var error = new Error("The grammar contains unreachable symbols.")
                .AddDetails("The unreachable productionsGroup", unreachableProductions.ToString())
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The unreachable productionsGroup", unreachableProductions)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        var leftRecursiveCicles = set.GetLeftRecursionCicles();

        if (leftRecursiveCicles.Length != 0)
        {
            var ciclesStr = string.Join(Environment.NewLine, leftRecursiveCicles.Select(x => x.ToString()));
            var error = new Error("The grammar contains left recursion cicles.")
                .AddDetails("The left recursion cicles", ciclesStr)
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The left recursion cicles", leftRecursiveCicles)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        var firstSetConflictingProductionSubsets = set.GetSubsetsGroupedByFirstSetConflicts();

        if (firstSetConflictingProductionSubsets.Length != 0)
        {
            var subsetsStr = string.Join(Environment.NewLine, firstSetConflictingProductionSubsets.Select(x => x.ToString()));
            var error = new Error("The grammar contains first set conflicts.")
                .AddDetails("The first set conflicting production subsets", subsetsStr)
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The first set conflicting production subsets", firstSetConflictingProductionSubsets)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        var firstFollowConflictingProductionSubsets = set.GetSubsetsGroupedByFirstFollowConflicts();

        if (firstFollowConflictingProductionSubsets.Length != 0)
        {
            var subsetsStr = string.Join(Environment.NewLine, firstFollowConflictingProductionSubsets.Select(x => x.ToString()));
            var error = new Error("The grammar contains first-follow set conflicts.")
                .AddDetails("The first-follow set conflicting production subsets", subsetsStr)
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The first-follow set conflicting production subsets", firstFollowConflictingProductionSubsets)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        return errors.ToArray();
    }

}
