using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Components;
using ModularSystem.Webql.Analysis.Components;

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
