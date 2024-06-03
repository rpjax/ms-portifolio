using ModularSystem.Core.TextAnalysis.Tokenization;

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

        if (token.Type != type)
        {
            return false;
        }
        if (value is not null && token.Value.ToString() != value)
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

        if (token.Type != type)
        {
            throw new Exception($"Expected token of type {type} but got {token.Type}. At position {token.Metadata.StartPosition}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
        }
        if (value is not null && token.Value.ToString() != value)
        {
            throw new Exception($"Expected {type} token with value {value} but got {token.Value}. At position {token.Metadata.StartPosition}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
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
    * consume literals
    */
    public Token ConsumeLiteral()
    {
        var token = Stream.Consume();

        var isLiteral = false
            || token.Type == TokenType.String
            || token.Type == TokenType.Integer
            || token.Type == TokenType.Float
            ;

        if (!isLiteral)
        {
            throw new Exception($"Expected token of type {TokenType.String}, {TokenType.Integer} or {TokenType.Float} but got {token.Type}. At position {token.Metadata.StartPosition}, line {token.Metadata.Line}, char {token.Metadata.Column}.");
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

}
