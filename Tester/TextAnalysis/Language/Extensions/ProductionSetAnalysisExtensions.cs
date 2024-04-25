using ModularSystem.Core.TextAnalysis.Language.Tools;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionSetAnalysisExtensions
{
    /*
        Analysis helpers.    
    */
    public static bool ContainsMacro(this ProductionSet set)
    {
        return set.Productions.Any(x => x.ContainsMacro());
    }

    public static bool ContainsUnitProduction(this ProductionSet set)
    {
        return set.Productions.Any(x => x.IsUnitProduction());
    }

    public static bool ContainsUnreachableProduction(this ProductionSet set)
    {
        return set.GetUnreachableProductions().Length != 0;
    }

    public static bool ContainsUnrealizableProduction(this ProductionSet set)
    {
        return new SymbolRealizabilityTool()
            .Execute(set).Length != 0;
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

        var unreachableProductions = set.Productions
            .Where(x => !reachableSymbols.Contains(x.Head))
            .ToArray();

        return unreachableProductions;
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
        var unrealizableSet = new ProductionSet(set.Start);

        foreach (var nonTerminal in unrealizableNonTerminals)
        {
            var productions = set.Lookup(nonTerminal)
                .ToArray();

            unrealizableSet.Add(productions);
        }

        return unrealizableSet;
    }

    public static LeftRecursionCicle[] GetLeftRecursionCicles(this ProductionSet set)
    {
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

    public static ProductionGroup GetCommonPrefixProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var nonTerminalProductions = set.Productions
            .GroupBy(x => x.Head)
            .ToArray();

        var commonPrefixProductions = new List<ProductionSet>();

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

            commonPrefixProductions.Add(commonPrefixProductionsGroup);
        }

        return commonPrefixProductions;
    }

    public static NonTerminal[] GetCommonPrefixProductionHeads(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var nonTerminalProductions = set.Productions
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
                This part gets the full set of productions for the given non-terminal.
                The alternative productions must be preserved.
            */

            var head = commonPrefixProductionsGroup.First().Head;
            
            if(!heads.Contains(head))
            {
                heads.Add(head);
            }
        }

        return heads.ToArray();
    }

    /*
     * Assertion helpers.
     */
    public static void EnsureNoMacros(this ProductionSet set)
    {
        if (ContainsMacro(set))
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
            var error = new Error("The grammar contains unrealizable productions.")
                .AddDetails("The unrealizable productions", unrealizableProductions.ToString())
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The unrealizable productions", unrealizableProductions)
                .AddJsonData("The production set", set)
                ;

            errors.Add(error);
        }

        var unreachableProductions = set.RemoveUnreachableProductions();

        if (unreachableProductions.Length != 0)
        {
            var error = new Error("The grammar contains unreachable symbols.")
                .AddDetails("The unreachable productions", unreachableProductions.ToString())
                .AddDetails("The production set", set.ToString())
                .AddJsonData("The unreachable productions", unreachableProductions)
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

        return errors.ToArray();
    }

}
