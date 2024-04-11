namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;

public enum ValueType
{
    Null,
    String,
    Bool,
    Number
}

public abstract class ValueToken : JsonToken
{
    public abstract ValueType ValueType { get; }
    public override JsonTokenType TokenType { get; } = JsonTokenType.Value;
}

public class NullToken : ValueToken
{
    public override ValueType ValueType { get; } = ValueType.Null;
}

public class StringToken : ValueToken
{
    public override ValueType ValueType { get; } = ValueType.String;
    public string Value { get; }

    public StringToken(string? value)
    {
        Value = value;
    }
}

public class BoolToken : ValueToken
{
    public override ValueType ValueType { get; } = ValueType.Bool;
    public bool Value { get; }

    public BoolToken(bool value)
    {
        Value = value;
    }
}

public class NumberToken : ValueToken
{
    public override ValueType ValueType { get; } = ValueType.Number;
    public string Value { get; }

    public NumberToken(string value)
    {
        Value = value;
    }

    public int Int32()
    {
        return int.Parse(Value);
    }

    public long Int64()
    {
        return long.Parse(Value);
    }

    public float Float32()
    {
        return float.Parse(Value);
    }

    public double Float64()
    {
        return double.Parse(Value);
    }

    public decimal Float128()
    {
        return decimal.Parse(Value);
    }
}
