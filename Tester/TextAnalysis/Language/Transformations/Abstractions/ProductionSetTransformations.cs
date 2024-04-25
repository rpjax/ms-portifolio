using System.Text;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/*
    separator.
*/

public class ProductionSetTransformation
{
    internal string? Explanation { get; set; }
    internal ProductionSetOperation[] Operations { get; set; }

    [JsonConstructor]
    public ProductionSetTransformation(string? explanation, params ProductionSetOperation[] operations)
    {
        Explanation = explanation;
        Operations = operations;
    }

    public ProductionSetTransformation(params ProductionSetOperation[] operations)
    {
        Operations = operations;
        Explanation = GetDefaultExplanation();
    }

    public ProductionSet Apply(ProductionSet set)
    {
        foreach (var operation in Operations)
        {
            set = operation.Apply(set);
        }

        return set;
    }

    public ProductionSet Reverse(ProductionSet set)
    {
        foreach (var operation in Operations.Reverse())
        {
            set = operation.Reverse(set);
        }

        return set;
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return GetDefaultExplanation();
    }

    internal ProductionSetTransformation AddOperation(params ProductionSetOperation[] operations)
    {
        Operations = Operations
            .Concat(operations)
            .ToArray();
        ;
        return this;
    }

    internal ProductionSetTransformation AddOperations(IEnumerable<ProductionSetOperation> operations)
    {
        Operations = Operations
            .Concat(operations)
            .ToArray();
        ;
        return this;
    }

    private string GetDefaultExplanation()
    {
        var builder = new StringBuilder();

        foreach (var operation in Operations)
        {
            builder.AppendLine(operation.ToString());
        }

        return builder.ToString();
    }
}

public class ProductionSetTransformationBuilder
{
    private List<ProductionSetOperation> Operations { get; }
    private string? Explanation { get; set; }

    public ProductionSetTransformationBuilder()
    {
        Operations = new();
    }

    public ProductionSetOperation[] GetOperations()
    {
        return Operations.ToArray();
    }

    public ProductionSetTransformationBuilder AddProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new AddProductionOperation(production));
        }

        return this;
    }

    public ProductionSetTransformationBuilder RemoveProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new RemoveProductionOperation(production));
        }

        return this;
    }

    public ProductionSetTransformationBuilder ReplaceSymbol(Symbol oldSymbol, Symbol newSymbol)
    {
        Operations.Add(new ReplaceSymbolOperation(oldSymbol, newSymbol));
        return this;
    }

    // public ProductionSetTransformationBuilder SetExplanation(string explanation)
    // {
    //     Explanation = explanation;
    //     return this;
    // }

    // public ProductionSetTransformation Build()
    // {
    //     return new ProductionSetTransformation(Operations.ToArray());
    // }
}
