using ModularSystem.Webql.Analysis.Components;

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

    public override AxiomSymbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAxiom(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        if(Lambda is not null)
        {
            yield return Lambda;
        }
    }
}
