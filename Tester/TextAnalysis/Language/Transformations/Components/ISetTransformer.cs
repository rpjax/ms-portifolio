using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public interface ISetTransformer
{
    SetTransformationCollection ExecuteTransformations(ProductionSet set);
}
