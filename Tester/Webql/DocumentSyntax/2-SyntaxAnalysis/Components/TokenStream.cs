using ModularSystem.Webql.Analysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

public class TokenStream
{
    public bool Eos { get; private set; }
    private IEnumerator<Token> Source;

    public TokenStream(IEnumerable<Token> source)
    {
        Source = source.GetEnumerator();
    }

    public void Dispose()
    {
        Source.Dispose();
    }

    public TokenStream Init()
    {
        Eos = !Source.MoveNext();
        return this;
    }

    public Token Peek()
    {
        if (Eos)
        {
            throw new InvalidOperationException("End of file reached");
        }

        return Source.Current;
    }

    public Token? TryPeek()
    {
        if (Eos)
        {
            return null;
        }

        return Source.Current;
    }

    public Token Consume()
    {
        if (Eos)
        {
            throw new InvalidOperationException("End of file reached");
        }

        var token = Source.Current;
        Eos = !Source.MoveNext();
        return token;
    }
}
