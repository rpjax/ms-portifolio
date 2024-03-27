namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaArgumentSymbol : Symbol
{
    public string Type { get; internal set; }
    public string Identifier { get; }

    public LambdaArgumentSymbol(string type, string identifier)
    {
        Type = type;
        Identifier = identifier;
    }

    public override string ToString()
    {
        return $"{Type} {Identifier}";
    }
}
