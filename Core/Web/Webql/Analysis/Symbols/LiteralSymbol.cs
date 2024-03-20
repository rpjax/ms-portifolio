namespace ModularSystem.Webql.Analysis.Symbols;

public abstract class LiteralSymbol : ArgumentSymbol
{

}

public class NullSymbol : LiteralSymbol
{
    public override string ToString()
    {
        return "null";
    }
}

public class StringSymbol : LiteralSymbol
{
    public string Value { get; }

    public StringSymbol(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return "\"Value\"";
    }
}

public class BoolSymbol : LiteralSymbol
{
    public bool Value { get; }

    public BoolSymbol(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class NumberSymbol : LiteralSymbol
{
    public string Value { get; }

    public NumberSymbol(string value)
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

    public override string ToString()
    {
        return Value;
    }
}
