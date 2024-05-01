using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class DuplicateProductionsRemoval : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        var ignoreSet = new List<ProductionRule>();

        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        var duplicates = set
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x.Skip(1))
            .ToArray();

        foreach (var duplicate in duplicates)
        {
            set.GetTransformationBuilder("Duplicate Production Removal")
                .RemoveProductions(duplicate)
                .Build();
        }

        return set.GetTrackedTransformations();
    }
}
