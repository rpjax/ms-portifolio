using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class TransformationRecordCollection : IEnumerable<ProductionTransformationRecord>
{
    private List<ProductionTransformationRecord> Records { get; }

    public TransformationRecordCollection(IEnumerable<ProductionTransformationRecord>? records = null)
    {
        Records = records?.ToList() ?? new();
    }

    public static implicit operator List<ProductionTransformationRecord>(TransformationRecordCollection set)
    {
        return set.Records;
    }

    public static implicit operator ProductionTransformationRecord[](TransformationRecordCollection set)
    {
        return set.Records.ToArray();
    }

    public static implicit operator TransformationRecordCollection(List<ProductionTransformationRecord> rewrites)
    {
        return new TransformationRecordCollection(rewrites);
    }

    public static implicit operator TransformationRecordCollection(ProductionTransformationRecord[] rewrites)
    {
        return new TransformationRecordCollection(rewrites.ToList());
    }

    public int Length => Records.Count;

    public IEnumerator<ProductionTransformationRecord> GetEnumerator()
    {
        return Records.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Records.GetEnumerator();
    }

    public TransformationRecordCollection Copy()
    {
        return new(this);
    }

    public TransformationRecordCollection Add(ProductionTransformationRecord rewrite)
    {
        Records.Add(rewrite);
        return this;
    }

    public TransformationRecordCollection Add(IEnumerable<ProductionTransformationRecord> rewrites)
    {
        Records.AddRange(rewrites);
        return this;
    }

    public override string ToString()
    {
        return string.Join("\n", Records.Select(x => x.ToString()));
    }
}