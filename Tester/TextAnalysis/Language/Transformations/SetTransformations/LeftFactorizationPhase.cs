using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/// <summary>
/// Represents a transformation phase that identifies and factors out all common prefixes in a production set.
/// </summary>
/// <remarks>
/// It differs from <see cref="LeftFactorization"/> in that it can operate on the entire set of productions.
/// </remarks>
public class LeftFactorizationPhase : SetTransformationPhase
{
    public LeftFactorizationPhase(ProductionSet set) : base(set)
    {
    }

    protected override SetTransformation[] GetTransformations(ProductionSet set)
    {
        var builder = new SetTransformationCollectionBuilder();

        foreach (var productionSet in set.GetCommonPrefixProductions())
        {
            builder.AddTransformation(new LeftFactorization(productionSet));
        }

        builder.SetExplanation(GetExplanation(builder.GetTransformations()));

        return builder.GetTransformations();
    }

    protected override string GetExplanation(IReadOnlyList<SetTransformation> transformations)
    {
        var transformationsStr = string.Join(Environment.NewLine, transformations.Select(x => x.ToString()));
        var explanation = $"Common prefix factorization phase:\n{transformationsStr}\n";

        return explanation;
    }
}

public class UnitProductionExpansion : SetTransformationPhase
{
    public UnitProductionExpansion(ProductionSet set) : base(set)
    {
    }

    // public UnitProductionExpansion(ProductionSet set)
    // {
    //     var rewrites = new List<ProductionTransformationRecord>();
    //     var ignoreSet = new List<ProductionRule>();

    //     while (true)
    //     {
    //         foreach (var production in set.Copy())
    //         {
    //             if (!production.IsUnitProduction())
    //             {
    //                 continue;
    //             }
    //             if (ignoreSet.Contains(production))
    //             {
    //                 continue;
    //             }
    //             if (production.Body[0] is not NonTerminal nonTerminal)
    //             {
    //                 throw new InvalidOperationException("The body of the production is not a nonterminal.");
    //             }

    //             var unitProduction = production;
    //             var unitHead = unitProduction.Head;
    //             var unitBody = nonTerminal;
    //             var replacementProductions = set.Lookup(unitBody).ToArray();
    //             var newProductions = new List<ProductionRule>();

    //             if (unitHead == unitBody)
    //             {
    //                 ignoreSet.Add(production);
    //                 continue;
    //             }

    //             var skip = false;

    //             foreach (var replacementProduction in replacementProductions)
    //             {
    //                 var newProduction = new ProductionRule(unitHead, replacementProduction.Body);

    //                 if (newProduction == unitProduction)
    //                 {
    //                     skip = true;
    //                     ignoreSet.Add(production);
    //                     break;
    //                 }

    //                 newProductions.Add(newProduction);
    //             }

    //             if (skip)
    //             {
    //                 continue;
    //             }

    //             set.Remove(unitProduction);
    //             set.Add(newProductions.ToArray());

    //             rewrites.Add(new ProductionTransformationRecord(production, newProductions, ProductionTransformationReason.UnitProductionExpansion));
    //         }

    //         var unitProductions = set
    //             .Where(x => x.IsUnitProduction())
    //             .Where(x => !ignoreSet.Contains(x))
    //             .ToArray();

    //         if (unitProductions.Length == 0)
    //         {
    //             break;
    //         }
    //     }

    //     Explanation = GetExplanation();
    // }

    protected override string GetExplanation(IReadOnlyList<SetTransformation> transformations)
    {
        var transformationsStr = string.Join(Environment.NewLine, transformations.Select(x => x.ToString()));
        return $"Unit productions expansion\n{transformationsStr}";
    }

    protected override SetTransformation[] GetTransformations(ProductionSet set)
    {
        var ignoreSet = new List<ProductionRule>();
        var transformations = new List<SetTransformation>();

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

            var builder = new SetTransformationBuilder()
                .RemoveProductions(unitProduction)
                .AddProductions(newProductions.ToArray())
                ;

            transformations.Add(builder.Build());
        }

        return transformations.ToArray();
    }
}
