using System.Text;

namespace Core.TextAnalysis;

// NOTE: tokens must relate to BNF concept of tokens.
// tokens have productions.
// tokens can be made of tokens or literals.
public abstract class LexerToken
{
    public string ProductionString => GetProductionString();
    public abstract string GetProductionString();
    public abstract override string ToString();
}

public class RawToken : LexerToken
{
    private string InternalProductionString { get; }
    private LexerToken[] Children { get; }

    public RawToken(string productionString, params LexerToken[] children)
    {
        InternalProductionString = productionString;
        Children = children;
    }

    public override string GetProductionString()
    {
        return InternalProductionString;
    }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();

        foreach (var token in Children)
        {
            strBuilder.Append(token.ToString());
        }

        return strBuilder.ToString();
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
