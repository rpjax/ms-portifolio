namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class AlternationMacro : ProductionMacro
{
    public override MacroType MacroType => MacroType.Alternation;

    private Sentence[] Alternatives { get; }

    public AlternationMacro(params Sentence[] alternatives)
    {
        Alternatives = alternatives;

        if (alternatives.Length == 0)
        {
            throw new ArgumentException("The alternation macro must contain at least one alternative.");
        }
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override bool Equals(object? obj)
    {
        return obj is AlternationMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var alternative in Alternatives)
            {
                hash = (hash * 16777619) ^ alternative.GetHashCode();
            }

            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is AlternationMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        return Alternatives;
    }

    public override string ToNotation(NotationType notation)
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
        return string.Join(" | ", Alternatives.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    private string ToBnfNotation()
    {
        return string.Join(" | ", Alternatives.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    private string ToEbnfNotation()
    {
        return string.Join(" | ", Alternatives.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    private string ToEbnfKleeneNotation()
    {
      return string.Join(" | ", Alternatives.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }
}

