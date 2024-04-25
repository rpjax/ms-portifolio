using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class ProductionSetTransformationCollection
{
    private string? Explanation { get; }
    private ProductionSetTransformation[] Transformations { get; set; }

    [JsonConstructor]
    public ProductionSetTransformationCollection(
        string? explanation,
        ProductionSetTransformation[] transformations)
    {
        Explanation = explanation;
        Transformations = transformations;
    }

    public static implicit operator ProductionSetTransformationCollection(ProductionSetTransformation[] transformations)
    {
        return new ProductionSetTransformationCollection(
            explanation: null,
            transformations: transformations
        );
    }

    public static implicit operator ProductionSetTransformationCollection(List<ProductionSetTransformation> transformations)
    {
        return new ProductionSetTransformationCollection(
            explanation: null,
            transformations: transformations.ToArray()
        );
    }

    public static implicit operator ProductionSetTransformation[](ProductionSetTransformationCollection collection)
    {
        return collection.Transformations;
    }

    public ProductionSetTransformationCollection Concat(ProductionSetTransformationCollection other)
    {
        return new ProductionSetTransformationCollection(
            explanation: null,
            transformations: Transformations.Concat(other.Transformations).ToArray()
        );
    }    

    public ProductionSet Apply(ProductionSet set)
    {
        foreach (var transformation in Transformations)
        {
            set = transformation.Apply(set);
        }

        return set;
    }

    public ProductionSet Reverse(ProductionSet set)
    {
        foreach (var transformation in Transformations.Reverse())
        {
            set = transformation.Reverse(set);
        }

        return set;
    }

}

public class ProductionSetTransformationCollectionBuilder
{
    private List<ProductionSetTransformation> Transformations { get; }
    private string? Explanation { get; set; }

    public ProductionSetTransformationCollectionBuilder()
    {
        Transformations = new();
    }

    public ProductionSetTransformationCollectionBuilder AddTransformation(ProductionSetTransformation transformation)
    {
        Transformations.Add(transformation);
        return this;
    }

    public ProductionSetTransformationCollectionBuilder SetExplanation(string explanation)
    {
        Explanation = explanation;
        return this;
    }

    public ProductionSetTransformation[] GetTransformations()
    {
        return Transformations.ToArray();
    }

    public ProductionSetOperation[] GetOperations()
    {
        return Transformations
            .SelectMany(x => x.Operations)
            .ToArray();
    }

    public ProductionSetTransformationCollection Build()
    {
        return new ProductionSetTransformationCollection(
            explanation: Explanation,
            transformations: Transformations.ToArray()
        );
    }

}

