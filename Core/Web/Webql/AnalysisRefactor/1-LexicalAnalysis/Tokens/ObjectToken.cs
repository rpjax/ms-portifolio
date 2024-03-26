using System.Collections;

namespace ModularSystem.Webql.Analysis.Tokens;

public class ObjectToken : Token, IEnumerable<ObjectPropertyToken>
{
    public override TokenType TokenType { get; } = TokenType.Object;
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
