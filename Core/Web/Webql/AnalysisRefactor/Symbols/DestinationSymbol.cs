namespace ModularSystem.Webql.Analysis.Symbols;

public class DestinationSymbol : Symbol
{
    public string? Value { get; }

    public DestinationSymbol(string? value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }
}
