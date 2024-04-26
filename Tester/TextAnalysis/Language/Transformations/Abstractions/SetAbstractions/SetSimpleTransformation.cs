using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public abstract class SetSimpleTransformation : SetTransformation
{
    public SetSimpleTransformation(ProductionRule production)
    {
        var operations = GetOperations(production);
        AddOperations(operations);
        Explanation = GetExplanation(operations);
    }

    protected abstract SetOperation[] GetOperations(ProductionRule production);
    protected abstract string GetExplanation(IReadOnlyList<SetOperation> operations);
}
