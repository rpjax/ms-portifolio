using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class TokenStreamConsumer
{
    private TokenStream Stream { get; }

    public TokenStreamConsumer(TokenStream stream)
    {
        Stream = stream;
    }
}
