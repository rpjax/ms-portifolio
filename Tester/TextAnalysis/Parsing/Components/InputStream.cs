using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public class InputStream : IDisposable
{
    private IEnumerator<Token> TokenStream { get; }
    private bool IsEndReached { get; set; }

    public InputStream(string input, Tokenizer tokenizer)
    {
        TokenStream = tokenizer.Tokenize(input).GetEnumerator();
        IsEndReached = !TokenStream.MoveNext();
    }

    public Terminal? Lookahead => Peek();

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    public Terminal? Peek()
    {
        if (IsEndReached)
        {
            return null;
        }

        return new Terminal(TokenStream.Current.Type, TokenStream.Current.Value);
    }

    public void Consume()
    {
        if (IsEndReached)
        {
            throw new InvalidOperationException("The end of the input stream has been reached.");
        }

        IsEndReached = !TokenStream.MoveNext();
    }
}