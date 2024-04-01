using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Visitors;

public class SourceArgumentFinder : AstSemanticVisitor
{
    private AxiomSymbol Axiom { get; }
    private DeclarationStatementSymbol? Argument { get; set; }

    public SourceArgumentFinder(AxiomSymbol axiom)
    {
        Axiom = axiom;
    }

    public DeclarationStatementSymbol GetSourceArgument()
    {
        if(Axiom.Lambda is null)
        {
            throw new Exception();
        }

        VisitLambdaExpression(new SemanticContext(), Axiom.Lambda);

        if (Argument is null)
        {
            throw new Exception();
        }

        return Argument;
    }

    protected override LambdaExpressionSymbol VisitLambdaExpression(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        Argument = symbol.Parameters[0];
        return symbol;
    }
}
