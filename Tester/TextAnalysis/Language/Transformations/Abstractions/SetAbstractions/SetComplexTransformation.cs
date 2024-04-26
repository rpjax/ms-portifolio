using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/// <summary>
/// Provides a framework for building set transformations.
/// </summary>
public abstract class SetComplexTransformation : SetTransformation
{
    public SetComplexTransformation(ProductionSet set)
    {
        var operations = GetOperations(set);
        AddOperations(operations);
        Explanation = GetExplanation(operations);
    }

    protected abstract SetOperation[] GetOperations(ProductionSet set);
    protected abstract string GetExplanation(IReadOnlyList<SetOperation> operations);
}
