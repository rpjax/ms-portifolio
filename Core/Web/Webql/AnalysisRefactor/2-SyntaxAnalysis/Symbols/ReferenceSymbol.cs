namespace ModularSystem.Webql.Analysis.Symbols;

public class ReferenceSymbol : ArgumentSymbol
{
    public string Value { get; }

    public ReferenceSymbol(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"{Value[1..]}";
    }
}
