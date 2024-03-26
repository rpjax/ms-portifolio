namespace ModularSystem.Webql.Analysis.Tokens;

public enum TokenType
{
    Value,
    Array,
    Object,
    ObjectProperty
}

public abstract class Token
{
    public abstract TokenType TokenType { get; }
}
