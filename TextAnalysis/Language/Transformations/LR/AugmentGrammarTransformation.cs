using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class AugmentGrammarTransformation : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        if (set.IsAugmented())
        {
            return set.GetTrackedTransformations();
        }

        var start = set.Start;
        var augmentedStart = new Sentence(start);

        if (set.Productions.Any(x => x.Head == start && x.Body == augmentedStart))
        {
            return set.GetTrackedTransformations();
        }

        var newStart = set.CreateNonTerminalPrime(start);

        var augmentedProduction = new ProductionRule(
            head: newStart,
            body: augmentedStart
        );

        set.GetTransformationBuilder("Augment Grammar")
            .AddProductions(augmentedProduction)
            .SetStart(newStart)
            .Build()
            ;
        
        return set.GetTrackedTransformations();
    }
}
