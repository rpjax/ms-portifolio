namespace ModularSystem.Webql.Analysis.Tokenization;

public enum TokenType
{
    Identifier,
    Keyword,
    Operator,
    Punctuation,

    /*
     * literals.
     */
    String,
    Integer,
    Float,
    Boolean,
    Null,
}

public class Token
{
    public TokenType TokenType { get; }
    public string Value { get; }
    public TokenMetadata Metadata { get; }

    public Token(TokenType tokenType, string value, TokenMetadata metadata)
    {
        TokenType = tokenType;
        Value = value;
        Metadata = metadata;
    }

    public override string ToString()
    {
        if (TokenType == TokenType.String)
        {
            return $"{TokenType}: {Value}";
        }
        else
        {
            return $"{TokenType}: \"{Value}\"";
        }
    }
}

public class TokenMetadata
{
    public int Position { get; }
    public int Line { get; }
    public int Column { get; }

    public TokenMetadata(int position, int line, int column)
    {
        Position = position;
        Line = line;
        Column = column;
    }
}
