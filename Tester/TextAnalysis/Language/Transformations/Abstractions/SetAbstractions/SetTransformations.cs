using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/*
    separator.
*/

public class SetTransformation : IEquatable<SetTransformation>, IEqualityComparer<SetTransformation>
{
    internal string? Explanation { get; set; }
    internal SetOperation[] Operations { get; set; }

    [JsonConstructor]
    public SetTransformation(string? explanation, params SetOperation[] operations)
    {
        Explanation = explanation;
        Operations = operations;
    }

    public SetTransformation(params SetOperation[] operations)
    {
        Operations = operations;
        Explanation = GetDefaultExplanation();
    }

    public static bool operator ==(SetTransformation left, SetTransformation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SetTransformation left, SetTransformation right)
    {
        return !left.Equals(right);
    }

    public static implicit operator SetTransformation(SetOperation[] transformations)
    {
        return new SetTransformation(transformations.ToArray());
    }

    public static implicit operator SetTransformation(List<SetOperation> transformations)
    {
        return new SetTransformation(transformations.ToArray());
    }

    public static implicit operator SetOperation[](SetTransformation transformation)
    {
        return transformation.Operations;
    }

    public bool Equals(SetTransformation? other)
    {
        if (other is null)
        {
            return false;
        }

        return Operations.SequenceEqual(other.Operations);
    }

    public bool Equals(SetTransformation? x, SetTransformation? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetTransformation);
    }

    public int GetHashCode([DisallowNull] SetTransformation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;

            foreach (var operation in Operations)
            {
                hash = hash * 23 + operation.GetHashCode();
            }

            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return GetDefaultExplanation();
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

    internal SetTransformation AddOperation(params SetOperation[] operations)
    {
        Operations = Operations
            .Concat(operations)
            .ToArray();
        ;
        return this;
    }

    internal SetTransformation AddOperations(IEnumerable<SetOperation> operations)
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

public class SetTransformationBuilder
{
    private List<SetOperation> Operations { get; }
    private string? Explanation { get; set; }

    public SetTransformationBuilder()
    {
        Operations = new();
    }

    public SetOperation[] GetOperations()
    {
        return Operations.ToArray();
    }

    public SetTransformationBuilder AddProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new AddProductionOperation(production));
        }

        return this;
    }

    public SetTransformationBuilder RemoveProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new RemoveProductionOperation(production));
        }

        return this;
    }

    public SetTransformationBuilder ReplaceSymbol(Symbol oldSymbol, Symbol newSymbol)
    {
        Operations.Add(new ReplaceSymbolOperation(oldSymbol, newSymbol));
        return this;
    }

    public SetTransformationBuilder SetExplanation(string explanation)
    {
        Explanation = explanation;
        return this;
    }

    public SetTransformation Build()
    {
        return new SetTransformation(Operations.ToArray());
    }
}
