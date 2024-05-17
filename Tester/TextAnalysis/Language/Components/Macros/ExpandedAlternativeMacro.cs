using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class ExpandedAlternativeMacro : ProductionMacro
{
    public override MacroType MacroType => MacroType.ExpandedAlternative;

    private Sentence[] Alternatives { get; }

    public ExpandedAlternativeMacro(params Sentence[] alternatives)
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
        return obj is ExpandedAlternativeMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override bool Equals(Symbol? x, Symbol? y)
    {
        return x is ExpandedAlternativeMacro macro1
            && y is ExpandedAlternativeMacro macro2
            && macro1.Alternatives.SequenceEqual(macro2.Alternatives);
    }

    public override bool Equals(Symbol? other)
    {
        return other is ExpandedAlternativeMacro macro
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

    public override int GetHashCode([DisallowNull] Symbol obj)
    {
        return obj.GetHashCode();
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

