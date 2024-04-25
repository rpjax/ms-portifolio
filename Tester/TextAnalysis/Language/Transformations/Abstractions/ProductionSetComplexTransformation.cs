using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public abstract class ProductionSetComplexTransformation : ProductionSetTransformation
{
    public ProductionSetComplexTransformation(ProductionSet set)
    {
        var operations = GetOperations(set);
        AddOperations(operations);
        Explanation = GetExplanation(operations);
    }

    protected abstract ProductionSetOperation[] GetOperations(ProductionSet set);
    protected abstract string GetExplanation(IReadOnlyList<ProductionSetOperation> operations);
}
