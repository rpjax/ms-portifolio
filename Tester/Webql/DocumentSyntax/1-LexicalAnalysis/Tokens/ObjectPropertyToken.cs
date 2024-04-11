namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public class ObjectPropertyToken : JsonToken
{
    public override JsonTokenType TokenType { get; } = JsonTokenType.ObjectProperty;
    public string Key { get; }
    public JsonToken Value { get; }

    public ObjectPropertyToken(string key, JsonToken value)
    {
        Key = key;
        Value = value;
    }
}
