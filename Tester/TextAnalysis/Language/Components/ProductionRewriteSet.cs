using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class ProductionRewriteSet : IEnumerable<ProductionRewrite>
{
    private List<ProductionRewrite> Rewrites { get; }

    public ProductionRewriteSet(List<ProductionRewrite>? rewrites = null)
    {
        Rewrites = rewrites ?? new();
    }

    public static implicit operator List<ProductionRewrite>(ProductionRewriteSet set)
    {
        return set.Rewrites;
    }

    public static implicit operator ProductionRewrite[](ProductionRewriteSet set)
    {
        return set.Rewrites.ToArray();
    }

    public static implicit operator ProductionRewriteSet(List<ProductionRewrite> rewrites)
    {
        return new ProductionRewriteSet(rewrites);
    }

    public static implicit operator ProductionRewriteSet(ProductionRewrite[] rewrites)
    {
        return new ProductionRewriteSet(rewrites.ToList());
    }

    public int Length => Rewrites.Count;

    public IEnumerator<ProductionRewrite> GetEnumerator()
    {
        return Rewrites.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Rewrites.GetEnumerator();
    }

    public void Add(ProductionRewrite rewrite)
    {
        Rewrites.Add(rewrite);
    }

    public void Add(IEnumerable<ProductionRewrite> rewrites)
    {
        Rewrites.AddRange(rewrites);
    }

    public override string ToString()
    {
        return string.Join("\n", Rewrites.Select(x => x.ToString()));
    }
}