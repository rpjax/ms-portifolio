using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class AstSemanticVisitorOld
{
    protected virtual AxiomSymbol VisitAxiom(SemanticContextOld context, AxiomSymbol symbol)
    {
        OnVisit(context, symbol);

        if (symbol.Lambda is null)
        {
            return symbol;
        }

        return new AxiomSymbol(
            lambda: VisitExpression(context, symbol.Lambda).As<LambdaExpressionSymbol>(context)
        );
    }

    protected virtual StatementBlockSymbol VisitStatementBlock(SemanticContextOld context, StatementBlockSymbol symbol)
    {
        OnVisit(context, symbol);

        for (int i = 0; i < symbol.Statements.Length; i++)
        {
            symbol.Statements[i] = VisitStatement(context, symbol.Statements[i]);
        }

        return symbol;
    }

    protected virtual StatementSymbol VisitStatement(SemanticContextOld context, StatementSymbol symbol)
    {
        OnVisit(context, symbol);

        switch (symbol.StatementType)
        {
            case StatementType.Expression:
                return VisitExpression(context, (ExpressionSymbol)symbol);

            case StatementType.Declaration:
                return VisitDeclaration(context, (DeclarationStatementSymbol)symbol);
        }

        throw new Exception();
    }

    protected virtual ExpressionSymbol VisitExpression(SemanticContextOld context, ExpressionSymbol symbol)
    {
        OnVisit(context, symbol);

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

    protected virtual DeclarationStatementSymbol VisitDeclaration(SemanticContextOld context, DeclarationStatementSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual ExpressionSymbol VisitLiteralExpression(SemanticContextOld context, LiteralExpressionSymbol symbol)
    {
        OnVisit(context, symbol);

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

    protected virtual ExpressionSymbol VisitReferenceExpression(SemanticContextOld context, ReferenceExpressionSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual ExpressionSymbol VisitOperatorExpression(SemanticContextOld context, OperatorExpressionSymbol symbol)
    {
        OnVisit(context, symbol);

        for (int i = 0; i < symbol.Operands.Length; i++)
        {
            symbol.Operands[i] = VisitExpression(context, symbol.Operands[i]);
        }

        return symbol;
    }

    protected virtual LambdaExpressionSymbol VisitLambdaExpression(SemanticContextOld context, LambdaExpressionSymbol symbol)
    {
        OnVisit(context, symbol);
        return new LambdaExpressionSymbol(
            parameters: symbol.Parameters
                .Select(x => VisitDeclaration(context, x))
                .ToArray(),
            body: VisitStatementBlock(context, symbol.Body)
        );
    }

    protected virtual AnonymousTypeExpressionSymbol VisitTypeProjectionExpression(
        SemanticContextOld context, 
        AnonymousTypeExpressionSymbol symbol
    )
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual NullSymbol VisitNull(SemanticContextOld context, NullSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual StringSymbol VisitString(SemanticContextOld context, StringSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual BoolSymbol VisitBool(SemanticContextOld context, BoolSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    protected virtual NumberSymbol VisitNumber(SemanticContextOld context, NumberSymbol symbol)
    {
        OnVisit(context, symbol);
        return symbol;
    }

    //*
    //* Hooks
    //*

    protected virtual void OnVisit(SemanticContextOld context, Symbol symbol)
    {

    }

}
