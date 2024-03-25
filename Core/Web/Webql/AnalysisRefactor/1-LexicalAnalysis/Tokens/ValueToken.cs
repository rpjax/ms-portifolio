namespace ModularSystem.Webql.Analysis.Tokens;

public abstract class ValueToken : Token
{

}

public class NullToken : ValueToken
{

}

public class StringToken : ValueToken
{
    public string Value { get; }

    public StringToken(string? value)
    {
        Value = value;
    }
}

public class BoolToken : ValueToken
{
    public bool Value { get; }

    public BoolToken(bool value)
    {
        Value = value;
    }
}

public class NumberToken : ValueToken
{
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
