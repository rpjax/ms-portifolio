namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class RepetitionMacro : SentenceMacro
{
    public override MacroType MacroType => MacroType.Repetition;

    public RepetitionMacro(params ProductionSymbol[] symbols) : base(symbols)
    {
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        yield return Sentence.Add(nonTerminal);
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
        return $"{{ {Sentence.ToNotation(NotationType.Sentential)} }}";
    }

    private string ToBnfNotation()
    {
        return $"{{ {Sentence.ToNotation(NotationType.Bnf)} }}";
    }

    private string ToEbnfNotation()
    {
        return $"{{ {Sentence.ToNotation(NotationType.Ebnf)} }}";
    }

    private string ToEbnfKleeneNotation()
    {
        return $"( {Sentence.ToNotation(NotationType.EbnfKleene)} )*";
    }
}

