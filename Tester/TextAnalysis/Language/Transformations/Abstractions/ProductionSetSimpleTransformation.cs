using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public abstract class ProductionSetSimpleTransformation : ProductionSetTransformation
{
    public ProductionSetSimpleTransformation(ProductionRule production)
    {
        var operations = GetOperations(production);
        AddOperations(operations);
        Explanation = GetExplanation(operations);
    }

    protected abstract ProductionSetOperation[] GetOperations(ProductionRule production);
    protected abstract string GetExplanation(IReadOnlyList<ProductionSetOperation> operations);
}
