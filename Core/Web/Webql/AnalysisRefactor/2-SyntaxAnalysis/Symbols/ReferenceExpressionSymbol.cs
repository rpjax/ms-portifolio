namespace ModularSystem.Webql.Analysis.Symbols;

public class ReferenceExpressionSymbol : ExpressionSymbol
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.Reference;
    private string Value { get; }

    public ReferenceExpressionSymbol(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return GetNormalizedValue();
    }

    public string GetNormalizedValue()
    {
        return $"{Value[1..]}";
    }
}
