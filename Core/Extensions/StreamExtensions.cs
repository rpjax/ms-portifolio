namespace ModularSystem.Core.Extensions;

public static class StreamExtensions
{
    public static MemoryStream FromString(this MemoryStream stream, string value)
    {
        var writer = new StreamWriter(stream);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
