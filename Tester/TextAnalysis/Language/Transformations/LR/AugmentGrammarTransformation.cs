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
        var eoi = Eoi.Instance;
        var augmentedStart = new Sentence(start, eoi);

        if (set.Productions.Any(x => x.Head == start && x.Body == augmentedStart))
        {
            return set.GetTrackedTransformations();
        }

        var newStart = set.CreateNonTerminalPrime(start);

        var newProduction = new ProductionRule(
            head: newStart,
            body: augmentedStart
        );

        set.GetTransformationBuilder("LR1AugmentStart Grammar")
            .AddProductions(newProduction)
            .SetStart(newStart)
            .Build()
            ;

        return set.GetTrackedTransformations();
    }
}
