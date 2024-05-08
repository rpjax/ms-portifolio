using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.LL1.Components;

public class LL1InputStream : IDisposable
{
    private IEnumerator<Token?> TokenStream { get; }

    public LL1InputStream(string input)
    {
        TokenStream = LL1Parser.Tokenizer.Tokenize(input).GetEnumerator();
        TokenStream.MoveNext();
    }

    public Terminal? Lookahead => Peek();

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    public Terminal? Peek()
    {
        if (TokenStream.Current == null)
        {
            return null;
        }

        return new Terminal(TokenStream.Current.Type, TokenStream.Current.Value);
    }

    public void Consume()
    {
        TokenStream.MoveNext();
    }
}
