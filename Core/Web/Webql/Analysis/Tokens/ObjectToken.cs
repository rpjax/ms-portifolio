using System.Collections;

namespace ModularSystem.Webql.Analysis.Tokens;

public class ObjectToken : Token, IEnumerable<ObjectProperty>
{
    public ObjectProperty[] Properties { get; }

    public ObjectToken(ObjectProperty[] properties)
    {
        Properties = properties;
    }

    public IEnumerator<ObjectProperty> GetEnumerator()
    {
        return Properties.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Properties.GetEnumerator();
    }
}

public class ObjectProperty
{
    public string Key { get; }
    public Token Value { get; }

    public ObjectProperty(string key, Token value)
    {
        Key = key;
        Value = value;
    }
}
