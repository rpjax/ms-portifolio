using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class ProductionSet : IEnumerable<ProductionRule>
{
    internal NonTerminal? Start { get; set; }
    internal List<ProductionRule> Productions { get; set; }

    public ProductionSet(params ProductionRule[] productions)
    {
        Start = productions.FirstOrDefault()?.Head;
        Productions = new(productions);
    }

    public ProductionSet(NonTerminal? start, params ProductionRule[] productions)
    {
        Start = start;
        Productions = new(productions);
    }

    public ProductionRule this[int index]
    {
        get => Productions[index];
        set => Productions[index] = value;
    }

    public static bool operator ==(ProductionSet left, ProductionSet right)
    {
        return left.SequenceEqual(right);
    }

    public static bool operator !=(ProductionSet left, ProductionSet right)
    {
        return !left.SequenceEqual(right);
    }

    public static implicit operator ProductionRule[](ProductionSet set)
    {
        return set.Productions.ToArray();
    }

    public static implicit operator ProductionSet(ProductionRule[] productions)
    {
        return new ProductionSet(productions);
    }

    public static implicit operator List<ProductionRule>(ProductionSet set)
    {
        return set.Productions;
    }

    public static implicit operator ProductionSet(List<ProductionRule> productions)
    {
        return new ProductionSet(productions.ToArray());
    }

    public static implicit operator ProductionSet(ProductionRule production)
    {
        return new ProductionSet(production);
    }

    public int Length => Productions.Count;

    public IEnumerator<ProductionRule> GetEnumerator()
    {
        return Productions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Productions.GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductionSet set
            && set.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var production in Productions)
            {
                hash = (hash * 16777619) ^ production.GetHashCode();
            }

            return hash;
        }
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public ProductionSet Copy()
    {
        return new ProductionSet(Start, Productions.ToArray());
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

    public IEnumerable<ProductionRule> Lookup(NonTerminal nonTerminal)
    {
        return Productions.Where(x => x.Head == nonTerminal);
    }

    public string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return ToSententialNotation();

            case NotationType.Bnf:
                return ToBnfNotation();

            case NotationType.Ebnf:
                return ToEbnfNotation();

            case NotationType.EbnfKleene:
                return ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    /*
     * notation conversion helper methods section.
    */

    private string ToSententialNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    private string ToBnfNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    private string ToEbnfNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    private string ToEbnfKleeneNotation()
    {
        return string.Join(Environment.NewLine, Productions.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }

}
