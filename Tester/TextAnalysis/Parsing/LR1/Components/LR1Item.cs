using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1Item : 
    IEquatable<LR1Item>, 
    IEqualityComparer<LR1Item>
{
    public ProductionRule Production { get; }
    public int Position { get; }
    public Terminal[] Lookaheads { get; }

    public LR1Item(ProductionRule production, int position, params Terminal[] lookaheads)
    {
        Production = production;
        Position = position;
        Lookaheads = lookaheads;
    }

    public static bool operator ==(LR1Item left, LR1Item right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LR1Item left, LR1Item right)
    {
        return !left.Equals(right);
    }

    public Symbol? Symbol => Position < Production.Body.Length 
        ? Production.Body[Position] 
        : null;

    public string Signature => GetSignature(useLookaheads: true);

    public bool Equals(LR1Item? other)
    {
        return other is not null 
            && Production.Equals(other.Production) 
            && Position == other.Position 
            && Lookaheads.SequenceEqual(other.Lookaheads);
    }

    public bool Equals(LR1Item? left, LR1Item? right)
    {
        return (left is not null && right is not null)
            && left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Item);
    }

    public int GetHashCode([DisallowNull] LR1Item obj)
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Production.GetHashCode();
            hash = hash * 23 + Position.GetHashCode();

            foreach (var lookahead in Lookaheads)
            {
                hash = hash * 23 + lookahead.GetHashCode();
            }

            return hash;
        }
    }

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }

    public override string ToString()
    {
        return GetSignature(useLookaheads: true);
    }

    public string GetSignature(bool useLookaheads = true)
    {
        var sentenceStrBuilder = new List<string>();

        for (int i = 0; i < Production.Body.Length; i++)
        {
            if (i == Position)
            {
                sentenceStrBuilder.Add("•");
            }

            sentenceStrBuilder.Add(Production.Body[i].ToString());
        }

        if (Position == Production.Body.Length)
        {
            sentenceStrBuilder.Add("•");
        }

        var sentenceStr = string.Join(" ", sentenceStrBuilder);
        var @base = $"{Production.Head} -> {sentenceStr}";

        var orderedLookaheads = Lookaheads.Length < 2
            ? Lookaheads
            : Lookaheads
                .OrderBy(x => x)
                .ToArray();

        var lookaheadStrs = orderedLookaheads
            .Select(x => x.ToString())
            .ToArray();

        var lookaheads = string.Join(", ", lookaheadStrs);

        if (useLookaheads)
        {
            return $"[({@base}), {{{lookaheads}}}]";
        }

        return $"[({@base})]";
    }

    public Sentence GetAlpha()
    {
        return Production.Body.GetRange(0, Position);
    }

    public Sentence GetBeta()
    {
        return Production.Body.GetRange(Position + 1, Production.Body.Length - Position);
    }

    public LR1Item GetNextItem()
    {
        if(Position >= Production.Body.Length)
        {
            throw new InvalidOperationException("The position is already at the end of the production body.");
        }

        return new LR1Item(Production, Position + 1, Lookaheads);
    }

}
