using ModularSystem.TextAnalysis.Language.Components;

namespace ModularSystem.TextAnalysis.Language.Transformations;

public interface ISetTransformer
{
    SetTransformationCollection ExecuteTransformations(ProductionSet set);
}
