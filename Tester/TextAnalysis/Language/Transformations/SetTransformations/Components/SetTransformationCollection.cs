using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class SetTransformationCollection : IEquatable<SetTransformationCollection>, IEqualityComparer<SetTransformationCollection>
{
    private SetTransformation[] Transformations { get; set; }

    [JsonConstructor]
    public SetTransformationCollection(params SetTransformation[] transformations)
    {
        Transformations = transformations;
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
            transformations: transformations
        );
    }

    public static implicit operator SetTransformationCollection(List<SetTransformation> transformations)
    {
        return new SetTransformationCollection(
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

        return Transformations.SequenceEqual(other.Transformations);
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
            transformations: Transformations.Concat(other.Transformations).ToArray()
        );
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var transformation in Transformations)
        {
            builder.AppendLine(transformation.ToString());
        }

        return builder.ToString();
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

}
