using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class SetTransformationCollection : IEquatable<SetTransformationCollection>, IEqualityComparer<SetTransformationCollection>
{
    internal string? Explanation { get; set;}
    private SetTransformation[] Transformations { get; set; }

    [JsonConstructor]
    public SetTransformationCollection(
        string? explanation,
        params SetTransformation[] transformations)
    {
        Explanation = explanation;
        Transformations = transformations;
    }

    public SetTransformationCollection(params SetTransformation[] transformations)
    {
        Transformations = transformations;
        Explanation = GetDefaultExplanation();
    }

    public static bool operator ==(SetTransformationCollection left, SetTransformationCollection right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SetTransformationCollection left, SetTransformationCollection right)
    {
        return !left.Equals(right);
    }

    public static implicit operator SetTransformationCollection(SetTransformation[] transformations)
    {
        return new SetTransformationCollection(
            explanation: null,
            transformations: transformations
        );
    }

    public static implicit operator SetTransformationCollection(List<SetTransformation> transformations)
    {
        return new SetTransformationCollection(
            explanation: null,
            transformations: transformations.ToArray()
        );
    }

    public static implicit operator SetTransformation[](SetTransformationCollection collection)
    {
        return collection.Transformations;
    }

    public bool Equals(SetTransformationCollection? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Explanation == other.Explanation
            && Transformations.SequenceEqual(other.Transformations);
    }

    public bool Equals(SetTransformationCollection? x, SetTransformationCollection? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetTransformationCollection);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ (Explanation?.GetHashCode() ?? 0);
            hash = (hash * 16777619) ^ Transformations.GetHashCode();
            return hash;
        }
    }

    public int GetHashCode([DisallowNull] SetTransformationCollection obj)
    {
        return obj.GetHashCode();
    }

    public SetTransformationCollection Concat(SetTransformationCollection other)
    {
        return new SetTransformationCollection(
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

    public override string ToString()
    {
        return Explanation ?? GetDefaultExplanation();
    }

    internal void AddTransformations(params SetTransformation[] transformations)
    {
        Transformations = Transformations
            .Concat(transformations)
            .ToArray();
    }

    internal void AddTransformations(IEnumerable<SetTransformation> transformations)
    {
        Transformations = Transformations
            .Concat(transformations)
            .ToArray();
    }

    private string GetDefaultExplanation()
    {
        var builder = new StringBuilder();

        builder.AppendLine("Set transformation collection:");

        foreach (var transformation in Transformations)
        {
            builder.AppendLine(transformation.ToString());
        }

        return builder.ToString();
    }

}

public class SetTransformationCollectionBuilder
{
    private List<SetTransformation> Transformations { get; }
    private string? Explanation { get; set; }

    public SetTransformationCollectionBuilder()
    {
        Transformations = new();
    }

    public SetTransformationCollectionBuilder AddTransformation(SetTransformation transformation)
    {
        Transformations.Add(transformation);
        return this;
    }

    public SetTransformationCollectionBuilder SetExplanation(string explanation)
    {
        Explanation = explanation;
        return this;
    }

    public SetTransformation[] GetTransformations()
    {
        return Transformations.ToArray();
    }

    public SetOperation[] GetOperations()
    {
        return Transformations
            .SelectMany(x => x.Operations)
            .ToArray();
    }

    public SetTransformationCollection Build()
    {
        return new SetTransformationCollection(
            explanation: Explanation,
            transformations: Transformations.ToArray()
        );
    }

}
