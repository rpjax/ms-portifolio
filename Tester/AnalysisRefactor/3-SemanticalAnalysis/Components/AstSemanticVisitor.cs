using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class AstSemanticVisitor
{
    protected virtual AxiomSymbol VisitAxiom(SemanticContext context, AxiomSymbol axiom)
    {
        if(axiom.Lambda is null)
        {
            return axiom;
        }

        return new AxiomSymbol(
            lambda: VisitExpression(context, axiom.Lambda).As<LambdaExpressionSymbol>(context)
        );
    }

    protected virtual StatementBlockSymbol VisitStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        for (int i = 0; i < symbol.Statements.Length; i++)
        {
            symbol.Statements[i] = VisitStatement(context, symbol.Statements[i]);
        }

        return symbol;
    }

    protected virtual StatementSymbol VisitStatement(SemanticContext context, StatementSymbol symbol)
    {
        switch (symbol.StatementType)
        {
            case StatementType.Expression:
                return VisitExpression(context, (ExpressionSymbol)symbol);

            case StatementType.Declaration:
                return VisitDeclaration(context, (DeclarationStatementSymbol)symbol);
        }

        throw new Exception();
    }

    protected virtual ExpressionSymbol VisitExpression(SemanticContext context, ExpressionSymbol symbol)
    {
        switch (symbol.ExpressionType)
        {
            case ExpressionType.Literal:
                return VisitLiteralExpression(context, (LiteralExpressionSymbol)symbol);

            case ExpressionType.Reference:
                return VisitReferenceExpression(context, (ReferenceExpressionSymbol)symbol);

            case ExpressionType.Operator:
                return VisitOperatorExpression(context, (OperatorExpressionSymbol)symbol);

            case ExpressionType.Lambda:
                return VisitLambdaExpression(context, (LambdaExpressionSymbol)symbol);

            case ExpressionType.AnonymousType:
                return VisitTypeProjectionExpression(context, (AnonymousTypeExpressionSymbol)symbol);
        }

        throw new Exception();
    }

    protected virtual DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        return symbol;
    }

    protected virtual ExpressionSymbol VisitLiteralExpression(SemanticContext context, LiteralExpressionSymbol symbol)
    {
        switch (symbol.LiteralType)
        {
            case LiteralType.Null:
                return VisitNull(context, (NullSymbol)symbol);

            case LiteralType.String:
                return VisitString(context, (StringSymbol)symbol);

            case LiteralType.Bool:
                return VisitBool(context, (BoolSymbol)symbol);

            case LiteralType.Number:
                return VisitNumber(context, (NumberSymbol)symbol);
        }

        throw new Exception();
    }

    protected virtual ExpressionSymbol VisitReferenceExpression(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        return symbol;
    }

    protected virtual ExpressionSymbol VisitOperatorExpression(SemanticContext context, OperatorExpressionSymbol symbol)
    {
        for (int i = 0; i < symbol.Operands.Length; i++)
        {
            symbol.Operands[i] = VisitExpression(context, symbol.Operands[i]);
        }

        return symbol;
    }

    protected virtual LambdaExpressionSymbol VisitLambdaExpression(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        return new LambdaExpressionSymbol(
            parameters: symbol.Parameters
                .Select(x => VisitDeclaration(context, x))
                .ToArray(),
            body: VisitStatementBlock(context, symbol.Body)
        );
    }

    protected virtual AnonymousTypeExpressionSymbol VisitTypeProjectionExpression(
        SemanticContext context, 
        AnonymousTypeExpressionSymbol symbol
    )
    {
        return symbol;
    }

    protected virtual NullSymbol VisitNull(SemanticContext context, NullSymbol symbol)
    {
        return symbol;
    }

    protected virtual StringSymbol VisitString(SemanticContext context, StringSymbol symbol)
    {
        return symbol;
    }

    protected virtual BoolSymbol VisitBool(SemanticContext context, BoolSymbol symbol)
    {
        return symbol;
    }

    protected virtual NumberSymbol VisitNumber(SemanticContext context, NumberSymbol symbol)
    {
        return symbol;
    }

}
