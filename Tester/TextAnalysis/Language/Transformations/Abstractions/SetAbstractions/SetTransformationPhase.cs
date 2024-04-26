using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/// <summary>
/// Provides a framework for building a collection of set transformations.
/// </summary>
public abstract class SetTransformationPhase : SetTransformationCollection
{
    public SetTransformationPhase(ProductionSet set)
    {
        var transformations = GetTransformations(set);
        AddTransformations(transformations);
        Explanation = GetExplanation(transformations);
    }

    protected abstract SetTransformation[] GetTransformations(ProductionSet set);
    protected abstract string GetExplanation(IReadOnlyList<SetTransformation> transformations);
}
