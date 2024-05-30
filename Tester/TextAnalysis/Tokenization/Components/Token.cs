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

/// <summary>
/// Represents the set of information associated with a token.
/// </summary>
public struct TokenMetadata
{
    /// <summary>
    /// Gets the character startPosition in the source text where the token starts. (0-based)
    /// </summary>
    public int StartPosition { get; }

    /// <summary>
    /// Gets the character startPosition in the source text where the token ends. (0-based)
    /// </summary>
    public int EndPosition { get; }

    /// <summary>
    /// Gets the line number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="TokenMetadata"/> struct.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    public TokenMetadata(int startPosition, int endPosition, int line, int column)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"line: {Line + 1}, column: {Column + 1}";
    }
}
