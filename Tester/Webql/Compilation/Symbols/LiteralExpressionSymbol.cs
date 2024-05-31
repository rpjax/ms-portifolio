using ModularSystem.Webql.Analysis.Components;

namespace ModularSystem.Webql.Analysis.Symbols;

public enum LiteralType
{
    Null,
    String,
    Bool,
    Number
}

public abstract class LiteralExpressionSymbol : ExpressionSymbol
{
    public abstract LiteralType LiteralType { get; }
    public override ExpressionType ExpressionType { get; } = ExpressionType.Literal;

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLiteralExpression(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        yield break;
    }
}

public class NullSymbol : LiteralExpressionSymbol
{
    public override LiteralType LiteralType { get; } = LiteralType.Null;

    public override string ToString()
    {
        return "null";
    }
}

public class StringSymbol : LiteralExpressionSymbol
{
    public override LiteralType LiteralType { get; } = LiteralType.String;
    public string? Value { get; }

    public StringSymbol(string? value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"\"{Value}\"";
    }

    public string GetNormalizedValue()
    {
        return Value ?? string.Empty;
    }
}

public class BoolSymbol : LiteralExpressionSymbol
{
    public override LiteralType LiteralType { get; } = LiteralType.Bool;
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

public class NumberSymbol : LiteralExpressionSymbol
{
    public override LiteralType LiteralType { get; } = LiteralType.Number;
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
