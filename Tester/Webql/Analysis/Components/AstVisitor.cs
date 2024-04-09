using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Components;

public enum AstNodeType
{
    Axiom,
    StatementBlock,
    DeclarationStatement,
    LiteralExpression,
    ReferenceExpression,
    OperatorExpression,
    LambdaExpression,
    AnonymousTypeExpression,
    TypeBinding
}

[Obsolete("Use AstRewriter instead.")]
public class AstVisitor : AstTraverser
{
    protected internal virtual AxiomSymbol VisitAxiom(AxiomSymbol symbol)
    {
        return symbol;
    }

    protected internal virtual StatementBlockSymbol VisitStatementBlock(StatementBlockSymbol symbol)
    {
        return symbol;
    }

    /*
     * statements.
     */

    protected internal virtual StatementSymbol VisitDeclarationStatement(DeclarationStatementSymbol symbol)
    {
        return symbol;
    }

    /*
     * expressions.
     */

    protected internal virtual ExpressionSymbol VisitLiteralExpression(LiteralExpressionSymbol symbol)
    {
        return symbol;
    }

    protected internal virtual ExpressionSymbol VisitReferenceExpression(ReferenceExpressionSymbol symbol)
    {
        return symbol;
    }

    protected internal virtual ExpressionSymbol VisitOperatorExpression(OperatorExpressionSymbol symbol)
    {
        return symbol;
    }

    protected internal virtual ExpressionSymbol VisitLambdaExpression(LambdaExpressionSymbol symbol)
    {
        return symbol;
    }

    protected internal virtual ExpressionSymbol VisitAnonymousTypeExpression(AnonymousTypeExpressionSymbol symbol)
    {
        return symbol;
    }

    /*
     * other symbols.
     */

    protected internal virtual TypeBindingSymbol VisitTypeBinding(TypeBindingSymbol symbol)
    {
        return symbol;
    }

    /*
     * helper methods.
     */

    private AstNodeType GetAstNodeType(Symbol symbol)
    {
        return symbol switch
        {
            AxiomSymbol _ => AstNodeType.Axiom,
            StatementBlockSymbol _ => AstNodeType.StatementBlock,
            DeclarationStatementSymbol _ => AstNodeType.DeclarationStatement,
            LiteralExpressionSymbol _ => AstNodeType.LiteralExpression,
            ReferenceExpressionSymbol _ => AstNodeType.ReferenceExpression,
            OperatorExpressionSymbol _ => AstNodeType.OperatorExpression,
            LambdaExpressionSymbol _ => AstNodeType.LambdaExpression,
            AnonymousTypeExpressionSymbol _ => AstNodeType.AnonymousTypeExpression,
            TypeBindingSymbol _ => AstNodeType.TypeBinding,
            _ => throw new NotImplementedException()
        };
    }

}
