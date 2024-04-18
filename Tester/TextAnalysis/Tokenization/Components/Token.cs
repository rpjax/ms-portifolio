namespace ModularSystem.Core.TextAnalysis.Tokenization;

public class Token
{
    public TokenType TokenType { get; }
    public string Value { get; }
    public TokenInfo Details { get; }

    public Token(TokenType tokenType, string value, TokenInfo metadata)
    {
        TokenType = tokenType;
        Value = value;
        Details = metadata;
    }

    public override string ToString()
    {
        if (TokenType == TokenType.String)
        {
            return $"{TokenType}: {Value} {Details}";
        }
        else
        {
            return $"{TokenType}: \"{Value}\"  {Details}";
        }
    }
}

public class TokenInfo
{
    public int Position { get; }
    public int Line { get; }
    public int Column { get; }

    public TokenInfo(int position, int line, int column)
    {
        Position = position;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"line: {Line + 1}, column: {Column + 1}";
    }
}
