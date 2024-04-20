namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// A production rule is a rule that defines how a non-terminal symbol can be replaced by a sequence of other symbols.
/// </summary>
public class ProductionRule
{
    // LHS
    public NonTerminal Head { get; }

    // RHS
    public Sentence Body { get; }

    public ProductionRule(NonTerminal head, params ProductionSymbol[] body)
    {
        Head = head;
        Body = body;
    }

    public ProductionRule(string name, params ProductionSymbol[] body)
    {
        Head = new NonTerminal(name);
        Body = body;
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
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
        var head = Head.ToNotation(NotationType.Sentential);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Sentential)));

        return $"{head} -> {body}";
    }

    private string ToBnfNotation()
    {
        var head = Head.ToNotation(NotationType.Bnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Bnf)));

        return $"{head} ::= {body}";
    }

    private string ToEbnfNotation()
    {
        var head = Head.ToNotation(NotationType.Ebnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Ebnf)));

        return $"{head} = {body}";
    }

    private string ToEbnfKleeneNotation()
    {
        var head = Head.ToNotation(NotationType.EbnfKleene);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.EbnfKleene)));

        return $"{head} = {body}";
    }

}
