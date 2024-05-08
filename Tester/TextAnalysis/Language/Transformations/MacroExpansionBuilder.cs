using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class MacroExpansion : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        set.ResetTransformationsTracker();

        while (set.ContainsMacro())
        {
            foreach (var production in set.Copy())
            {
                if (!production.ContainsMacro())
                {
                    continue;
                }

                var expandedProductions = production.ExpandMacros().ToArray();

                set.GetTransformationBuilder("Macro Expansion")
                    .RemoveProductions(production)
                    .AddProductions(expandedProductions)
                    .Build();
            }
        }

        return set.GetTrackedTransformations();
    }

}


