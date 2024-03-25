using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class AstSemanticVisitor
{
    public virtual Symbol Visit(SemanticContext context, Symbol symbol)
    {
        if (symbol is AxiomSymbol axiomSymbol)
        {
            return VisitAxiom(context, axiomSymbol);
        }
        if (symbol is LambdaSymbol lambdaSymbol)
        {
            return VisitLambda(context, lambdaSymbol);
        }

        throw new Exception();
    }

    public virtual AxiomSymbol VisitAxiom(SemanticContext context, AxiomSymbol axiom)
    {
        if(axiom.Lambda is null)
        {
            return axiom;
        }

        return new AxiomSymbol(VisitLambda(context, axiom.Lambda));
    }

    public virtual LambdaSymbol VisitLambda(SemanticContext context, LambdaSymbol symbol)
    {
        return new LambdaSymbol(
            arguments: VisitLambdaArguments(context, symbol.Arguments),
            body: VisitStatementBlock(context, symbol.Body)
        );
    }

    public virtual LambdaArgumentsSymbol VisitLambdaArguments(SemanticContext context, LambdaArgumentsSymbol symbol)
    {
        for (int i = 0; i < symbol.Arguments.Length; i++)
        {
            symbol.Arguments[i] = VisitLambdaArgument(context, symbol.Arguments[i]);
        }

        return symbol;
    }

    public virtual LambdaArgumentSymbol VisitLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
    {
        return symbol;
    }

    public virtual StatementBlockSymbol VisitStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        for (int i = 0; i < symbol.Statements.Length; i++)
        {
            symbol.Statements[i] = VisitStatement(context, symbol.Statements[i]);
        }

        return symbol;
    }

    public virtual StatementSymbol VisitStatement(SemanticContext context, StatementSymbol symbol)
    {
        return symbol;
    }
}
