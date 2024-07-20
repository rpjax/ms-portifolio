namespace ModularSystem.TextAnalysis;

/// <summary>
/// Encapsulates a stream of text, allowing for peeking(n) chars without consuming from the stream.
/// </summary>
public class TextSource : IDisposable
{
    private StreamReader StreamReader { get; }
    private char[] Buffer { get; set; }
    private int Position { get; set; }
    private int AvailableChars { get; set; }
    private bool ReachedEnd { get; set; }

    public TextSource(Stream stream, uint bufferSize = 1024)
    {
        StreamReader = new(stream);
        Position = 0;
        Buffer = new char[bufferSize];
    }

    public void Dispose()
    {
        StreamReader.Dispose();
    }

    public TextSource Init()
    {
        Peek(Buffer.Length);
        return this;
    }

    public void Consume(int count)
    {
        Position += count;
        AvailableChars -= count;
    }

    public char[] Peek(int count)
    {
        if (count > Buffer.Length)
        {
            throw new ArgumentException("Count cannot be greater than the buffer size.", nameof(count));
        }

        if (AvailableChars < count)
        {
            ReadFromStream();
        }
        if (AvailableChars <= 0)
        {
            return Array.Empty<char>();
        }

        var segmentSize = count;

        if (AvailableChars < count)
        {
            segmentSize = AvailableChars;
        }

        var chars = new char[segmentSize];
        Array.ConstrainedCopy(Buffer, Position, chars, 0, segmentSize);
        return chars;
    }

    public string PeekString(int length)
    {
        return string.Join("", Peek(length));
    }

    private void ReadFromStream()
    {
        if (ReachedEnd)
        {
            return;
        }

        CompressBuffer();

        var count = Buffer.Length - AvailableChars;
        var offset = AvailableChars;
        var readCount = StreamReader.Read(Buffer, offset, count);

        if (readCount == 0)
        {
            ReachedEnd = true;
            return;
        }

        Position = 0;
        AvailableChars += readCount;
    }

    private void CompressBuffer()
    {
        Array.ConstrainedCopy(Buffer, Position, Buffer, 0, AvailableChars);
    }
}