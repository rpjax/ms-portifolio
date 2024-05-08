using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class OptionMacro : SentenceMacro
{
    public override MacroType MacroType => MacroType.Option;

    public OptionMacro(params Symbol[] symbols) : base(symbols)
    {
    }

    public override int GetHashCode([DisallowNull] Symbol obj)
    {
        return obj.GetHashCode();
    }

    public override bool Equals(Symbol? x, Symbol? y)
    {
        return x is OptionMacro optionMacro 
            && y is OptionMacro otherOptionMacro 
            && optionMacro.Sentence.SequenceEqual(otherOptionMacro.Sentence);
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        yield return Sentence;
        yield return new Sentence(new Epsilon());
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
        return $"[ {Sentence.ToNotation(NotationType.Sentential)} ]";
    }

    private string ToBnfNotation()
    {
        return $"[ {Sentence.ToNotation(NotationType.Bnf)} ]";
    }

    private string ToEbnfNotation()
    {
        return $"[ {Sentence.ToNotation(NotationType.Ebnf)} ]";
    }

    private string ToEbnfKleeneNotation()
    {
        return $"( {Sentence.ToNotation(NotationType.EbnfKleene)} )?";
    }

}

