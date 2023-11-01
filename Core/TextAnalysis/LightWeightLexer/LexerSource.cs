using ModularSystem.Core;

namespace Core.TextAnalysis;

public abstract class LexerSource : IDisposable
{
    public abstract IEnumerable<char> AsEnumerable();

    public static LexerSource FromString(string str)
    {
        return new StreamLexerSource(str.ToMemoryStream());
    }

    public virtual void Dispose()
    {
        
    }
}

public class StreamLexerSource : LexerSource
{
    private Stream Stream { get; }

    public StreamLexerSource(Stream stream)
    {
        Stream = stream;
    }

    public override void Dispose()
    {
        Stream.Dispose();
    }

    public override IEnumerable<char> AsEnumerable()
    {
        using var reader = new StreamReader(Stream);
        var str = reader.ReadToEndAsync().Result;
        return str;
    }

}