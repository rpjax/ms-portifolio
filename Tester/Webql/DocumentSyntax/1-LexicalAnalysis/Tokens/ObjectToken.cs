using System.Collections;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public class ObjectToken : JsonToken, IEnumerable<ObjectPropertyToken>
{
    public override JsonTokenType TokenType { get; } = JsonTokenType.Object;
    public ObjectPropertyToken[] Properties { get; }

    public ObjectToken(ObjectPropertyToken[] properties)
    {
        Properties = properties;
    }

    public IEnumerator<ObjectPropertyToken> GetEnumerator()
    {
        return Properties.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Properties.GetEnumerator();
    }
}
