namespace ModularSystem.Webql.Analysis.Symbols;

public class AxiomSymbol : Symbol
{
    public LambdaSymbol? Lambda { get; }

    public AxiomSymbol(LambdaSymbol? lambda)
    {
        Lambda = lambda;
    }

    public override string ToString()
    {
        return Lambda?.ToString() ?? string.Empty;
    }
}
