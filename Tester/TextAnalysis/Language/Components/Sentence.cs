using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

/*
    Helper constructs.
*/

public class Sentence : IEnumerable<ProductionSymbol>
{
    public int Length => Symbols.Count;

    private List<ProductionSymbol> Symbols { get; }

    public Sentence(params ProductionSymbol[] symbols)
    {
        Symbols = new(symbols);
        ExpandPipeMacros();
    }

    public ProductionSymbol this[int index]
    {
        get => Symbols[index];
        set => Symbols[index] = value;
    }

    public static bool operator ==(Sentence left, Sentence right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Sentence left, Sentence right)
    {
        return !left.Equals(right);
    }

    public static implicit operator ProductionSymbol[](Sentence sentence)
    {
        return sentence.Symbols.ToArray();
    }

    public static implicit operator List<ProductionSymbol>(Sentence sentence)
    {
        return sentence;
    }

    public static implicit operator Sentence(ProductionSymbol[] productions)
    {
        return new Sentence(productions);
    }

    public static implicit operator Sentence(List<ProductionSymbol> productions)
    {
        return new Sentence(productions.ToArray());
    }

    // copilot, generate implicit conversion from and to productionSymbol array and list

    public IEnumerator<ProductionSymbol> GetEnumerator()
    {
        return Symbols.GetEnumerator();
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

    public Sentence Copy()
    {
        return new Sentence(Symbols.ToArray());
    }

    /// <summary>
    /// Gets the index of the specified symbol in the sentence. If the symbol is not found, -1 is returned. <br/>
    /// Equality is determined by reference equality. So, two symbols are considered equal if they are the same object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public int IndexOfSymbol(ProductionSymbol symbol)
    {
        var index = -1;

        foreach (var item in this)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    /*
        read helper methods section.
     */

    public ProductionSymbol GetLeftmostSymbol()
    {
        return this.First();
    }

    public ProductionSymbol GetRightmostSymbol()
    {
        return this.Last();
    }

    public NonTerminal? GetLeftmostNonTerminal()
    {
        return this
            .OfType<NonTerminal>()
            .FirstOrDefault();
    }

    public NonTerminal? GetRightmostNonTerminal()
    {
        return this
            .OfType<NonTerminal>()
            .LastOrDefault();
    }

    public Terminal? GetLeftmostTerminal()
    {
        return this
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public Terminal? GetRightmostTerminal()
    {
        return this
            .OfType<Terminal>()
            .LastOrDefault();
    }

    /*
        derivation helper methods section.
     */

    public Derivation Derive(int index, ProductionRule production)
    {
        if (index < 0 || index >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The index is out of range.");
        }

        if (this[index] is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException("The symbol at the specified index is not a non-terminal symbol. Derivations can only be performed on non-terminal symbols.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The non-terminal at the specified index does not match the head of the production rule.");
        }

        var derivedSentence = Copy()
            .RemoveAt(index)
            .InsertAt(index, production.Body);

        return new Derivation(
            production: production,
            originalSentence: this,
            nonTerminal: nonTerminal,
            derivedSentence: derivedSentence
        );
    }

    public Derivation DeriveLeftmostNonTerminal(ProductionRule production)
    {
        var nonTerminal = GetLeftmostNonTerminal();

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The leftmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(index, production);
    }

    public Derivation DeriveRightmostNonTerminal(ProductionRule production)
    {
        var nonTerminal = GetRightmostNonTerminal();

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The rightmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(index, production);
    }

    /*
        helper methods section.
     */

    public Sentence Add(ProductionSymbol symbol)
    {
        Symbols.Add(symbol);
        return this;
    }

    public Sentence Add(IEnumerable<ProductionSymbol> symbols)
    {
        Symbols.AddRange(symbols);
        return this;
    }

    public Sentence InsertAt(int index, ProductionSymbol symbol)
    {
        Symbols.Insert(index, symbol);
        return this;
    }

    public Sentence InsertAt(int index, IEnumerable<ProductionSymbol> symbols)
    {
        Symbols.InsertRange(index, symbols);
        return this;
    }

    public Sentence RemoveAt(int index)
    {
        Symbols.RemoveAt(index);
        return this;
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
        if (Symbols.All(x => x is not PipeMacro))
        {
            return;
        }

        var pipeIndexes = Symbols
            .Select((x, i) => (x, i))
            .Where(x => x.x is PipeMacro)
            .Select(x => x.i)
            .ToList();

        var alternatives = new List<Sentence>();
        var start = 0;

        pipeIndexes.Add(Symbols.Count);

        foreach (var index in pipeIndexes)
        {
            var end = index;
            var length = end - start;

            var alternative = new Sentence(Symbols.Skip(start).Take(length).ToArray());
            alternatives.Add(alternative);

            start = end + 1;
        }

        var alternationMacro = new AlternationMacro(alternatives.ToArray());

        Symbols.Clear();
        Symbols.Add(alternationMacro);
    }

}

