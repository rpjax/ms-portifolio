using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/*
    Helper constructs.
*/

public class Sentence : 
    IEnumerable<Symbol>, IEquatable<Sentence>, IEqualityComparer<Sentence>
{
    public int Length => Symbols.Length;

    internal Symbol[] Symbols { get; set; }

    public Sentence(params Symbol[] symbols)
    {
        Symbols = Array.Empty<Symbol>()
            .Concat(symbols)
            .ToArray();

        ExpandPipeMacros();
    }

    public Sentence(Symbol symbol, params Symbol[] symbols)
    {
        Symbols = Array.Empty<Symbol>()
            .Append(symbol)
            .Concat(symbols)
            .ToArray();
    }

    public Symbol this[int index]
    {
        get => Symbols[index];
    }

    public static bool operator ==(Sentence left, Sentence right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Sentence left, Sentence right)
    {
        return !left.Equals(right);
    }

    public static implicit operator Symbol[](Sentence sentence)
    {
        return sentence.Symbols.ToArray();
    }

    public static implicit operator List<Symbol>(Sentence sentence)
    {
        return sentence;
    }

    public static implicit operator Sentence(Symbol[] productions)
    {
        return new Sentence(productions);
    }

    public static implicit operator Sentence(List<Symbol> productions)
    {
        return new Sentence(productions.ToArray());
    }

    // copilot, generate implicit conversion from and to productionSymbol array and list

    public IEnumerator<Symbol> GetEnumerator()
    {
        return Symbols.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Symbols.GetEnumerator();
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override bool Equals(object? obj)
    {
        return obj is Sentence sentence
            && sentence.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var symbol in this)
            {
                hash = (hash * 16777619) ^ symbol.GetHashCode();
            }

            return hash;
        }
    }

    public bool Equals(Sentence? other)
    {
        return other is not null
            && other.SequenceEqual(this);
    }

    public bool Equals(Sentence? x, Sentence? y)
    {
        return x is not null
            && y is not null
            && x.SequenceEqual(y);
    }

    public int GetHashCode([DisallowNull] Sentence obj)
    {
        return obj.GetHashCode();
    }

    public Sentence Copy()
    {
        return new Sentence(Symbols.ToArray());
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

    private string ToSententialNotation()
    {
        return string.Join(" ", this.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    private string ToBnfNotation()
    {
        return string.Join(" ", this.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    private string ToEbnfNotation()
    {
        return string.Join(" ", this.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    private string ToEbnfKleeneNotation()
    {
        return string.Join(" ", this.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }

    private void ExpandPipeMacros()
    {
        if (Symbols.All(x => x is not AlternativeMacro))
        {
            return;
        }

        var pipeIndexes = Symbols
            .Select((x, i) => (x, i))
            .Where(x => x.x is AlternativeMacro)
            .Select(x => x.i)
            .ToList();

        var alternatives = new List<Sentence>();
        var start = 0;

        pipeIndexes.Add(Symbols.Length);

        foreach (var index in pipeIndexes)
        {
            var end = index;
            var length = end - start;

            var alternative = new Sentence(Symbols.Skip(start).Take(length).ToArray());
            alternatives.Add(alternative);

            start = end + 1;
        }

        var alternationMacro = new ExpandedAlternativeMacro(alternatives.ToArray());

        Symbols = new Symbol[1];
        Symbols[0] = alternationMacro;
    }
}
