namespace ModularSystem.Webql.Analysis.Symbols;

public class AxiomSymbol : Symbol
{
    public LambdaExpressionSymbol? Lambda { get; }

    public AxiomSymbol(LambdaExpressionSymbol? lambda)
    {
        Lambda = lambda;
    }

    public override string ToString()
    {
        return Lambda?.ToString() ?? string.Empty;
    }
}
