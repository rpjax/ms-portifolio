namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public enum JsonTokenType
{
    Value,
    Array,
    Object,
    ObjectProperty
}

public abstract class JsonToken
{
    public abstract JsonTokenType TokenType { get; }
}
