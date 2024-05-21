using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1State : IEquatable<LR1State>, IEqualityComparer<LR1State>
{
    public LR1Item[] Kernel { get; }
    public LR1Item[] Closure { get; }
    public LR1Item[] Items { get; }

    public LR1State(LR1Item[] kernel, LR1Item[] closure)
    {
        if(kernel.Length == 0)
        {
            throw new ArgumentException("The kernel array must not be empty.");
        }

        Kernel = kernel;
        Closure = closure;

        Items = kernel
            .Concat(closure)
            .ToArray();
    }

    public LR1State(LR1Item kernel, LR1Item[] closure) : this(new[] { kernel }, closure)
    {
    }

    public LR1Item this[int index] => Items[index];

    public string Signature => GetSignature();
    public bool IsFinalState => GetIsFinalState();

    public override string ToString()
    {
        var kernelStr = string.Join("\n", Kernel.Select(x => x.ToString()));
        var closureStr = string.Join("\n", Closure.Select(x => x.ToString()));

        return $"Kernel:\n{kernelStr}\n Closure:\n{closureStr}";
    }

    public bool IsAcceptingState(ProductionSet set)
    {
        if(!IsFinalState)
        {
            return false;
        }

        var augmentedProduction = set.TryGetAugmentedStartProduction();

        if(augmentedProduction is null)
        {
            throw new InvalidOperationException("The production set does not have an augmented production.");
        }

        return Kernel[0].Production == augmentedProduction;
    }

    private string GetSignature(bool useLookaheads = true)
    {
        var signatures = Kernel
            .Select(x => x.GetSignature(useLookaheads))
            .ToArray();
        
        return string.Join("; ", signatures);
    }

    private bool GetIsFinalState()
    {
        return Kernel.Any(item => item.Symbol is null);
    }

    public bool Equals(LR1State? left, LR1State? right)
    {
        return left?.GetSignature(useLookaheads:true) == right?.GetSignature(useLookaheads:true);
    }

    public int GetHashCode([DisallowNull] LR1State obj)
    {
        return obj.GetHashCode();
    }

    public bool Equals(LR1State? other)
    {
        return other is not null 
            && Equals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1State);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            
            foreach (var item in Kernel)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }
}