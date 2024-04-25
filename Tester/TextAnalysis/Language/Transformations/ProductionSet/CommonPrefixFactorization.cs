using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public static class CommonPrefixFactorization 
{
    public static ProductionSetTransformationCollection FromSet(ProductionSet set)
    {
        var builder = new ProductionSetTransformationCollectionBuilder();

        foreach (var productionSet in set.GetCommonPrefixProductions())
        {
            builder.AddTransformation(new LeftFactorization(productionSet));      
        }

        builder.SetExplanation(GetExplanation(builder.GetTransformations()));

        return builder.Build();
    }

    private static string GetExplanation(ProductionSetTransformation[] transformations)
    {

        var explanation = @$"";

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

        Explanation = GetExplanation();
    }

    private string GetExplanation()
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
