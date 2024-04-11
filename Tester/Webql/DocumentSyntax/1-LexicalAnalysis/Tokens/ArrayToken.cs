using System.Collections;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public class ArrayToken : JsonToken, IEnumerable<JsonToken>
{
    public override JsonTokenType TokenType { get; } = JsonTokenType.Array;
    public JsonToken[] Values { get; }

    public ArrayToken(JsonToken[] values)
    {
        Values = values;
    }

    public IEnumerator<JsonToken> GetEnumerator()
    {
        return Values.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }
}
