using ModularSystem.Webql.Analysis.Components;
using ModularSystem.Webql.Analysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class ParsingContext : IDisposable
{
    public bool Eos { get; private set; }
    public TokenStreamConsumer Consumer { get; private set; }

    private TokenStream TokenStream { get; }

    public ParsingContext(IEnumerable<Token> source)
    {
        TokenStream = new TokenStream(source).Init();
        Consumer = new TokenStreamConsumer(TokenStream);
    }

    public ParsingContext(TokenStream tokenStream)
    {
        TokenStream = tokenStream;
        Consumer = new TokenStreamConsumer(TokenStream);
    }

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    public TokenStreamConsumer CreateConsumer()
    {
        return new TokenStreamConsumer(TokenStream);
    }

}

public abstract class DS_Symbol
{
    public void Accept(AstRewriter visitor)
    {
        throw new NotImplementedException();
    }
}

public class DS_Statement : DS_Symbol
{
}

public class DS_Block
{
    public DS_Statement[] Statements { get; }

    public DS_Block(DS_Statement[] statements)
    {
        Statements = statements;
    }
}

public class BlockParser
{
    public static DS_Block ParseBlock(ParsingContext context)
    {
        var stream = context.CreateConsumer();
        var statements = new List<DS_Statement>();

        stream.ConsumePunctuation(LexicalAlphabet.OpenCurlyBracket);

        while (!stream.MatchPunctuation(LexicalAlphabet.CloseCurlyBracket))
        {
            statements.Add(StatementParser.ParseStatement(context));

            if(stream.MatchPunctuation(LexicalAlphabet.Comma))
            {
                stream.ConsumePunctuation(LexicalAlphabet.Comma);
            }
        }

        stream.ConsumePunctuation(LexicalAlphabet.CloseCurlyBracket);

        return new DS_Block(statements.ToArray());
    }
}

public class StatementParser
{
    public static DS_Statement ParseStatement(ParsingContext context)
    {
        var consumer = context.CreateConsumer();

        var keyToken = consumer.ConsumeStringLiteral();

        consumer.ConsumePunctuation(LexicalAlphabet.Colon);

        var valueToken = consumer.Consume();

        switch (valueToken.TokenType)
        {
            case TokenType.Identifier:
                break;
            case TokenType.Keyword:
                break;
            case TokenType.Operator:
                break;
            case TokenType.Punctuation:
                break;
            case TokenType.String:
                break;
            case TokenType.Integer:
                break;
            case TokenType.Float:
                break;
            case TokenType.Boolean:
                break;
            case TokenType.Null:
                break;
            default:
                break;
        }

        return new DS_Statement();
    }
}
