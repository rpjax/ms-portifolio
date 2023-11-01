namespace Core.TextAnalysis;

public class LiteralToken : LexerToken
{
    protected string Value { get; }

    public LiteralToken(string value)
    {
        Value = value;
    }

    public LiteralToken(char @char) : this($"{@char}")
    {

    }

    public override string GetProductionString()
    {
        return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}

public abstract class ValueToken : LexerToken
{
    private LexerToken Value { get; }

    public ValueToken(LexerToken value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

// <digit> ::= ...
// <letter> ::= ...
// <identifier> ::= ...
// <number> ::=
// <operator> ::= ...
// <term> ::= <identifier> | <number>
// <expression> ::= <term> | <expression> "+" <term>
// "5 + 5" => "<digit><ignore><operator><ignore><digit>"
// 4 * sizeof(char) | (4 * sizeof(char)) + (4 * sizeof(object))
// "1024" => "<digit><digit><digit><digit>" => "<number>"
