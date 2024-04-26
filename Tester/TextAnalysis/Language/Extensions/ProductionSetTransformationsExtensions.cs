using System.Text.Json;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Tools;
using ModularSystem.Core.TextAnalysis.Language.Transformations;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static partial class ProductionSetTransformationsExtensions
{
    /*
        Main API methods.
    */
    public static TransformationRecordCollection AutoClean(this ProductionSet set)
    {
        var rewrites = new List<ProductionTransformationRecord>()
            .Concat(set.ExpandMacros())
            .Concat(set.RemoveUnreachableProductions())
            .Concat(set.ExpandUnitProductions())
            .Concat(set.RemoveDuplicates())
            ;

        return rewrites.ToArray();
    }

    public static TransformationRecordCollection RecursiveAutoClean(this ProductionSet set)
    {
        var rewriteSet = new TransformationRecordCollection();

        while (true)
        {
            var rewrites = set.AutoClean();

            if (rewrites.Length == 0)
            {
                break;
            }

            rewriteSet.Add(rewrites);
        }

        return rewriteSet;
    }

    public static TransformationRecordCollection AutoFix(this ProductionSet set)
    {
        var rewrites = new List<ProductionTransformationRecord>()
            .Concat(set.RemoveLeftRecursion())
            .Concat(set.FactorCommonPrefixProductions())
            .ToArray()
            ;

        var errors = set.GetErrors();

        if (errors.Length > 0)
        {
            throw new ErrorException(errors);
        }

        return rewrites.ToArray();
    }

    public static TransformationRecordCollection RecursiveAutoFix(this ProductionSet set)
    {
        var transformations = new TransformationRecordCollection();

        while (true)
        {
            var iterationTransformations = new TransformationRecordCollection()
                .Add(set.AutoFix());

            if (iterationTransformations.Length == 0)
            {
                break;
            }

            transformations.Add(iterationTransformations);
        }

        return transformations;
    }

    public static TransformationRecordCollection AutoTransform(this ProductionSet set)
    {
        var transformations = new TransformationRecordCollection();

        while (true)
        {
            var iterationTransformations = new TransformationRecordCollection()
                .Add(set.RecursiveAutoClean())
                .Add(set.RecursiveAutoFix())
                ;

            if (iterationTransformations.Length == 0)
            {
                break;
            }

            transformations.Add(iterationTransformations);
        }

        return transformations;
    }

    /*
     * Specific Manipulation Operations.
     */
    public static TransformationRecordCollection ExpandMacros(this ProductionSet set)
    {
        var rewrites = new List<ProductionTransformationRecord>();

        while (set.ContainsMacro())
        {
            foreach (var production in set.Productions.ToArray())
            {
                if (production.ContainsMacro())
                {
                    var expandedProductions = production.ExpandMacros().ToArray();
                    var rewrite = new ProductionTransformationRecord(production, expandedProductions, ProductionTransformationReason.MacroExpansion);

                    set.Productions.Remove(production);
                    set.Productions.AddRange(expandedProductions);
                    rewrites.Add(rewrite);
                }
            }
        }

        return rewrites.ToArray();
    }

    public static int LeftFactor(this ProductionSet set)
    {
        set.EnsureNoMacros();

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

            var nonRecursiveProductions = productions
                .Where(productions => !productions.IsLeftRecursive())
                .ToArray();

            var alphas = recursiveProductions
                .Select(x => x.Body.Skip(1).ToArray())
                .ToArray();

            var betas = nonRecursiveProductions
                .Select(x => x.Body.ToArray())
                .ToArray();

            set.Remove(productions);

            var newNonTerminal = new NonTerminal(nonTerminal.Name + "′");

            foreach (var beta in betas)
            {
                var body = new List<Symbol>()
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
                    body = new Symbol[] { newNonTerminal };
                }

                set.Add(nonTerminal, body);
            }

            foreach (var alpha in alphas)
            {
                var body = new List<Symbol>()
                    .FluentAdd(alpha)
                    .FluentAdd(newNonTerminal)
                    .ToArray();

                set.Add(newNonTerminal, body);
            }

            set.Add(newNonTerminal, new Epsilon());
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

    public static TransformationRecordCollection RemoveLeftRecursion(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var rewrites = new List<ProductionTransformationRecord>();
        var recursiveBranches = new LeftRecursionTool()
            .Execute(set);

        while (recursiveBranches.Length != 0)
        {
            foreach (var branch in recursiveBranches)
            {
                if (branch.Root.Symbol is not NonTerminal rootSymbol)
                {
                    throw new InvalidOperationException("The root symbol is not a nonterminal.");
                }

                var recursiveProduction = branch.Nodes
                    .First(node => node.Production is not null && node.Production.Body.First() == branch.Root.Symbol).Production;

                recursiveProduction = branch.Nodes.Last().Production;

                if (recursiveProduction is null)
                {
                    throw new InvalidOperationException("The recursive production is null.");
                }

                var recursiveSymbol = recursiveProduction.Head;

                if (recursiveProduction.Body.First() != rootSymbol)
                {
                    throw new InvalidOperationException("The recursive symbol is not the first symbol in the recursive production.");
                }

                /*
                 * get all the productions of the root symbol that don't start with the recursive symbol. 
                 * remove the original recursive production.
                 * replace the recursive production with a new set productions, replacing the root symbol with the body of the productions
                 * 
                 * Ex:
                 * S -> A b 
                 * A -> B c.
                 * D -> S e.
                 * 
                 * Turns to:
                 * S -> A b.
                 * A -> B c.
                 * D -> A b e. (D -> S e. replaced with D -> A b e.)
                 */

                var productions = set.Lookup(rootSymbol)
                    .Where(production => production.Body.First() != recursiveSymbol)
                    .ToArray();

                if (productions.Length == 0)
                {
                    throw new InvalidOperationException("The grammar is not in the correct form.");
                }

                var suffix = recursiveProduction.Body.Skip(1).ToArray();
                var replacements = new List<ProductionRule>();

                set.Remove(recursiveProduction);

                foreach (var item in productions)
                {
                    var newProduction = new ProductionRule(
                        head: recursiveSymbol,
                        body: item.Body.Concat(suffix).ToArray()
                    );

                    set.Add(newProduction);
                    replacements.Add(newProduction);
                }

                rewrites.Add(new ProductionTransformationRecord(
                    originalProduction: recursiveProduction,
                    replacements: replacements,
                    reason: ProductionTransformationReason.LeftRecursionExpansion
                ));
            }

            recursiveBranches = new LeftRecursionTool()
                .Execute(set);
        }

        return rewrites.ToArray();
    }

    public static TransformationRecordCollection RemoveDuplicates(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var rewrites = new List<ProductionTransformationRecord>();
        var duplicates = set.Productions
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x.Skip(1))
            .ToArray();

        foreach (var duplicate in duplicates)
        {
            set.Productions.Remove(duplicate);
            rewrites.Add(new ProductionTransformationRecord(duplicate, null, ProductionTransformationReason.DuplicateProductionRemoval));
        }

        return rewrites.ToArray();
    }

    public static TransformationRecordCollection RemoveUnreachableProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

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

        // remove unreachable productions
        set.Productions = set.Productions
            .Where(x => reachable.Contains(x.Head))
            .ToList();

        // return the removed productions
        var rewrites = set.Productions
            .Where(x => !reachable.Contains(x.Head))
            .Select(x => new ProductionTransformationRecord(x, null, ProductionTransformationReason.UnreachableSymbolRemoval))
            .ToArray();

        return rewrites;
    }

    public static TransformationRecordCollection ExpandUnitProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var rewrites = new List<ProductionTransformationRecord>();
        var ignoreSet = new List<ProductionRule>();

        while (true)
        {
            var op = new UnitProductionExpansion(set);
            foreach (var production in set.Copy())
            {
                if (!production.IsUnitProduction())
                {
                    continue;
                }
                if (ignoreSet.Contains(production))
                {
                    continue;
                }
                if (production.Body[0] is not NonTerminal nonTerminal)
                {
                    throw new InvalidOperationException("The body of the production is not a nonterminal.");
                }

                var unitProduction = production;
                var unitHead = unitProduction.Head;
                var unitBody = nonTerminal;
                var replacementProductions = set.Lookup(unitBody).ToArray();
                var newProductions = new List<ProductionRule>();

                if (unitHead == unitBody)
                {
                    ignoreSet.Add(production);
                    continue;
                }

                var skip = false;

                foreach (var replacementProduction in replacementProductions)
                {
                    var newProduction = new ProductionRule(unitHead, replacementProduction.Body);

                    if (newProduction == unitProduction)
                    {
                        skip = true;
                        ignoreSet.Add(production);
                        break;
                    }

                    newProductions.Add(newProduction);
                }

                if (skip)
                {
                    continue;
                }

                set.Remove(unitProduction);
                set.Add(newProductions.ToArray());

                rewrites.Add(new ProductionTransformationRecord(production, newProductions, ProductionTransformationReason.UnitProductionExpansion));
            }

            var strOld = string.Join("\n", rewrites.Select(x => x.OriginalProduction.ToString()));
            var strNew = op.ToString();

            var unitProductions = set
                .Where(x => x.IsUnitProduction())
                .Where(x => !ignoreSet.Contains(x))
                .ToArray();

            if (unitProductions.Length == 0)
            {
                break;
            }
        }

        return rewrites.ToArray();
    }

    public static SetTransformationCollection FactorCommonPrefixProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var transformations = new TransformationRecordCollection();

        var commonPrefixProductionsSets = set.GetCommonPrefixProductions();

        foreach (var commonPrefixProductionSet in commonPrefixProductionsSets)
        {
            var commonPrefix = commonPrefixProductionSet[0].Body[0];
            var nonTerminal = commonPrefixProductionSet[0].Head;

            var newNonTerminal = set.CreateNonTerminalPrime(nonTerminal);

            var adjustedProduction = new ProductionRule(
                head: nonTerminal,
                body: new Sentence(commonPrefix, newNonTerminal)
            );

            var newNonTerminalProductions = new ProductionSet();

            foreach (var production in commonPrefixProductionSet)
            {
                var alpha = commonPrefix;
                var beta = production.Body.Skip(1).ToArray();

                var newProduction = new ProductionRule(
                    head: newNonTerminal,
                    body: beta
                );

                newNonTerminalProductions.Add(newProduction);
            }

            set.Remove(commonPrefixProductionSet);
            set.Add(adjustedProduction);
            set.Add(newNonTerminalProductions);

            foreach (var production in commonPrefixProductionSet)
            {
                var transformation = new ProductionTransformationRecord(
                    originalProduction: production,
                    replacements: adjustedProduction,
                    reason: ProductionTransformationReason.CommonPrefixFactorization
                );

                transformations.Add(transformation);
            }
        }

        return transformations;
    }

}
