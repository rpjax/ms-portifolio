namespace ModularSystem.Core.TextAnalysis.Tokenization;

/// <summary>
/// Represents a token produced by the lexical analyser, using a lexeme production rule.
/// </summary>
public class Token
{
    /// <summary>
    /// Gets the type of the token. (e.g. Identifier, Number, String, etc.)
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// Gets the value of the token. (e.g. "if", "123", "Hello World", etc.)
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the information associated with the token. (e.g. line number, column number, etc.)
    /// </summary>
    public TokenMetadata Metadata { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="value"></param>
    /// <param name="metadata"></param>
    public Token(TokenType tokenType, string value, TokenMetadata metadata)
    {
        Type = tokenType;
        Value = value;
        Metadata = metadata;
    }

    public override string ToString()
    {
        if(Value is null)
        {
            return Type.ToString();
        }

        return Value;
    }

    public string ToStringVerbose()
    {
        if (Type == TokenType.String)
        {
            return $"{Type}: {Value} {Metadata}";
        }
        else
        {
            return $"{Type}: \"{Value}\"  {Metadata}";
        }
    }

}
