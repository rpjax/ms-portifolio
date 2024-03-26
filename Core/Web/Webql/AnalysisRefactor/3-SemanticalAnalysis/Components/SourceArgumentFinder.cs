using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

public class SourceArgumentFinder : AstSemanticVisitor
{
    private AxiomSymbol Axiom { get; }
    private LambdaArgumentSymbol? Argument { get; set; }

    public SourceArgumentFinder(AxiomSymbol axiom)
    {
        Axiom = axiom;
    }

    public LambdaArgumentSymbol GetSourceArgument()
    {
        if(Axiom.Lambda is null)
        {
            throw new Exception();
        }

        VisitLambda(new SemanticContext(), Axiom.Lambda);

        if (Argument is null)
        {
            throw new Exception();
        }

        return Argument;
    }

    protected override LambdaSymbol VisitLambda(SemanticContext context, LambdaSymbol symbol)
    {
        Argument = symbol.Arguments[0];
        return symbol;
    }
}
