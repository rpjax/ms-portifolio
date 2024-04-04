namespace ModularSystem.Webql.Analysis.Symbols;

public class DestinationSymbol : Symbol
{
    public string? Value { get; }

    public DestinationSymbol(StringSymbol symbol)
    {
        Value = symbol.Value;
    }

    public DestinationSymbol(NullSymbol symbol)
    {
        Value = null;
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }
}
