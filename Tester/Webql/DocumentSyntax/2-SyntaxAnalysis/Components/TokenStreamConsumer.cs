using ModularSystem.Webql.Analysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class TokenStreamConsumer
{
    private TokenStream Stream { get; }

    public TokenStreamConsumer(TokenStream stream)
    {
        Stream = stream;
    }

    /*
     * matching methods.
     */

    public bool Match(TokenType type, string? value)
    {
        if(Stream.Eos)
        {
            return false;
        }

        var token = Stream.Peek();

        if (token.TokenType != type)
        {
            return false;
        }
        if (value is not null && token.Value != value)
        {
            return false;
        }

        return true;
    }

    public bool CharMatch(TokenType type, char? c)
    {
        return Match(type, c?.ToString());
    }

    /*
     * keywork match
     */

    public bool MatchKeyword(string? value)
    {
        return Match(TokenType.Keyword, value);
    }

    /*
     * punctuation match
     */

    public bool MatchPunctuation(string? value)
    {
        return Match(TokenType.Punctuation, value);
    }

    public bool MatchPunctuation(char c)
    {
        return CharMatch(TokenType.Punctuation, c);
    }

    /*
    * consume methods.
    */
    public Token Consume()
    {
        return Stream.Consume();
    }

    public Token Consume(TokenType type, string? value)
    {
        var token = Stream.Consume();

        if (token.TokenType != type)
        {
            throw new Exception($"Expected token of type {type} but got {token.TokenType}. At position {token.Metadata.Position}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
        }
        if (value is not null && token.Value != value)
        {
            throw new Exception($"Expected {type} token with value {value} but got {token.Value}. At position {token.Metadata.Position}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
        }

        return token;
    }

    public Token CharConsume(TokenType type, char? c)
    {
        return Consume(type, c?.ToString());
    }

    /*
     * consume identifier 
     */
    public Token ConsumeIdentifier(string? value)
    {
        return Consume(TokenType.Identifier, value);
    }

    public Token ConsumeIdentifier(char? c)
    {
        return CharConsume(TokenType.Identifier, c);
    }

    /*
    * consume keyword
    */
    public Token ConsumeKeyword(string value)
    {
        return Consume(TokenType.Keyword, value);
    }

    public Token ConsumeKeyword(char? c)
    {
        return CharConsume(TokenType.Keyword, c);
    }

    /*
    * consume punctuation
    */
    public Token ConsumePunctuation(string? value)
    {
        return Consume(TokenType.Punctuation, value);
    }

    public Token ConsumePunctuation(char? c)
    {
        return CharConsume(TokenType.Punctuation, c);
    }

    /*
    * consume operator
    */
    public Token ConsumeOperator(string? value)
    {
        return Consume(TokenType.Operator, value);
    }

    public Token ConsumeOperator(char? c)
    {
        return CharConsume(TokenType.Operator, c);
    }

    /*
    * consume literals
    */
    public Token ConsumeLiteral()
    {
        var token = Stream.Consume();

        var isLiteral = false
            || token.TokenType == TokenType.String
            || token.TokenType == TokenType.Integer
            || token.TokenType == TokenType.Float
            || token.TokenType == TokenType.Boolean
            || token.TokenType == TokenType.Null;

        if (!isLiteral)
        {
            throw new Exception($"Expected token of type {TokenType.String}, {TokenType.Integer}, {TokenType.Float}, {TokenType.Boolean} or {TokenType.Null} but got {token.TokenType}. At position {token.Metadata.Position}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
        }

        return token;
    }

    public Token ConsumeStringLiteral()
    {
        return Consume(TokenType.String, null);
    }

    public Token ConsumeIntegerLiteral()
    {
        return Consume(TokenType.Integer, null);
    }

    public Token ConsumeFloatLiteral()
    {
        return Consume(TokenType.Float, null);
    }

    public Token ConsumeBooleanLiteral()
    {
        return Consume(TokenType.Boolean, null);
    }

    public Token ConsumeNullLiteral()
    {
        return Consume(TokenType.Null, null);
    }

}
