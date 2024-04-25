using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class CommonPrefixFactorization : ProductionSetTransformation
{
    public CommonPrefixFactorization(ProductionSet set)
    {
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

            var newNonTerminalProductionSet = new ProductionSet();

            foreach (var production in commonPrefixProductionSet)
            {
                var alpha = commonPrefix;
                var beta = production.Body.Skip(1).ToArray();

                var newProduction = new ProductionRule(
                    head: newNonTerminal,
                    body: beta
                );

                newNonTerminalProductionSet.Add(newProduction);
            }

            var builder = new ProductionSetTransformationBuilder()
                .RemoveProductions(commonPrefixProductionSet)
                .AddProductions(adjustedProduction)
                .AddProductions(newNonTerminalProductionSet)
                ;

            AddOperations(builder.GetOperations());
        }
    }

    public override string GetExplanation()
    {
        var removeOperations = Operations
            .Where(x => x.Type == ProductionSetOperationType.RemoveProduction)
            .ToArray();

        var addOperations = Operations
            .Where(x => x.Type == ProductionSetOperationType.AddProduction)
            .ToArray();

        var removedProductions = removeOperations
            .Select(x => x.AsRemoveProduction().Production)
            .ToArray();

        var addedProductions = addOperations
            .Select(x => x.AsAddProduction().Production)
            .ToArray();

        var removedProductionsStr = string
            .Join(", ", removedProductions.Select(x => $"({x})"));

        var addedProductionsStr = string
            .Join(", ", addedProductions.Select(x => $"({x})"));

        var commonPrefix = removedProductions[0].Body[0];
        var newNonTerminal = addedProductions.Last().Head;

        var explanation = @$"The 'Common Prefix Factorization' transformation was applied to productions that start with {commonPrefix}. These productions were: [{removedProductionsStr}]. They initially caused non-determinism in parsers with a single lookahead due to the shared starting symbol. To address this issue, a new non-terminal symbol, {newNonTerminal}, was introduced. This symbol now encapsulates the sequences that follow {commonPrefix} in these productions. As a result, the productions were reorganized into: [{addedProductionsStr}]. This reorganization ensures that each production starts uniquely, eliminating the non-determinism at this parsing stage and making the grammar compatible with single lookahead parsers.";

        return explanation;
    }
}

public class UnitProductionExpansion : ProductionSetTransformation
{
    public UnitProductionExpansion(ProductionSet set)
    {
        var rewrites = new List<ProductionTransformationRecord>();
        var ignoreSet = new List<ProductionRule>();

        while (true)
        {
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

            var unitProductions = set
                .Where(x => x.IsUnitProduction())
                .Where(x => !ignoreSet.Contains(x))
                .ToArray();

            if (unitProductions.Length == 0)
            {
                break;
            }
        }
    }

    public override string GetExplanation()
    {
        var removeOperations = Operations
            .Where(x => x.Type == ProductionSetOperationType.RemoveProduction)
            .ToArray();

        var addOperations = Operations
            .Where(x => x.Type == ProductionSetOperationType.AddProduction)
            .ToArray();

        var removedProductions = removeOperations
            .Select(x => x.AsRemoveProduction().Production)
            .ToArray();

        var addedProductions = addOperations
            .Select(x => x.AsAddProduction().Production)
            .ToArray();

        var removedProductionsStr = string
            .Join(", ", removedProductions.Select(x => $"({x})"));

        var addedProductionsStr = string
            .Join(", ", addedProductions.Select(x => $"({x})"));

        var explanation = @$"The 'Unit Production Expansion' transformation was applied to unit productions. These productions were: [{removedProductionsStr}]. They were initially causing non-determinism in parsers with a single lookahead due to the shared starting symbol. To address this issue, the unit production was expanded into the productions: [{addedProductionsStr}]. This expansion ensures that the grammar is compatible with single lookahead parsers.";

        return explanation;
    }
}
