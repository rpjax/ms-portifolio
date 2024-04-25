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

    public ProductionRule(NonTerminal head, params Symbol[] body)
    {
        Head = head;
        Body = body;
    }

    public ProductionRule(string head, params Symbol[] body)
    {
        Head = new NonTerminal(head);
        Body = body;
    }

    public static bool operator ==(ProductionRule left, ProductionRule right)
    {
        return left.Head == right.Head && left.Body == right.Body;
    }

    public static bool operator !=(ProductionRule left, ProductionRule right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductionRule rule && rule == this;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Head.GetHashCode();
            hash = hash * 23 + Body.GetHashCode();

            return hash;
        }
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

    public ProductionRule Copy()
    {
        return new ProductionRule(Head.Copy(), Body.Copy());
    }

    private string ToSententialNotation()
    {
        var head = Head.ToNotation(NotationType.Sentential);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Sentential)));

        return $"{head} -> {body}.";
    }

    private string ToBnfNotation()
    {
        var head = Head.ToNotation(NotationType.Bnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Bnf)));

        return $"{head} ::= {body} ;";
    }

    private string ToEbnfNotation()
    {
        var head = Head.ToNotation(NotationType.Ebnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Ebnf)));

        return $"{head} = {body} ;";
    }

    private string ToEbnfKleeneNotation()
    {
        var head = Head.ToNotation(NotationType.EbnfKleene);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.EbnfKleene)));

        return $"{head} = {body} ;";
    }
}
