namespace ModularSystem.Webql.Analysis.Tokens;

public class ObjectPropertyToken : Token
{
    public override TokenType TokenType { get; } = TokenType.ObjectProperty;
    public string Key { get; }
    public Token Value { get; }

    public ObjectPropertyToken(string key, Token value)
    {
        Key = key;
        Value = value;
    }
}
