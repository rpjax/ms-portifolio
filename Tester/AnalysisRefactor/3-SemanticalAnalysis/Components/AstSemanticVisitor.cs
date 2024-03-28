using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class AstSemanticVisitor
{
    protected virtual Symbol Visit(SemanticContext context, Symbol symbol)
    {
        if (symbol is AxiomSymbol axiomSymbol)
        {
            return VisitAxiom(context, axiomSymbol);
        }
        if (symbol is LambdaExpressionSymbol lambdaSymbol)
        {
            return VisitLambda(context, lambdaSymbol);
        }
        if (symbol is DeclarationStatementSymbol declarationSymbol)
        {
            return VisitDeclaration(context, declarationSymbol);
        }
        if (symbol is StatementBlockSymbol statementBlock)
        {
            return VisitStatementBlock(context, statementBlock);
        }
        if (symbol is StatementSymbol statement)
        {
            return VisitStatement(context, statement);
        }
        if (symbol is ExpressionSymbol expression)
        {
            return VisitExpression(context, expression);
        }

        throw new Exception();
    }

    protected virtual AxiomSymbol VisitAxiom(SemanticContext context, AxiomSymbol axiom)
    {
        if(axiom.Lambda is null)
        {
            return axiom;
        }

        return new AxiomSymbol(VisitLambda(context, axiom.Lambda));
    }

    protected virtual LambdaExpressionSymbol VisitLambda(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        return new LambdaExpressionSymbol(
            parameters: symbol.Parameters
                .Select(x => Visit(context, x).As<DeclarationStatementSymbol>(context))
                .ToArray(),
            body: Visit(context, symbol.Body).As<StatementBlockSymbol>(context)
        );
    }

    protected virtual DeclarationStatementSymbol VisitDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        return symbol;
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
                return VisitExpression(context, (OperatorExpressionSymbol)symbol);

            default:
                break;
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
                return VisitReference(context, (ReferenceExpressionSymbol)symbol);

            case ExpressionType.Operator:
                return VisitOperatorExpression(context, (OperatorExpressionSymbol)symbol);

            default:
                break;
        }

        throw new Exception();
    }

    protected virtual LiteralExpressionSymbol VisitLiteralExpression(SemanticContext context, LiteralExpressionSymbol symbol)
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

            default:
                break;
        }

        throw new Exception();
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

    protected virtual ReferenceExpressionSymbol VisitReference(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        return symbol;
    }

    protected virtual ExpressionSymbol VisitOperatorExpression(SemanticContext context, OperatorExpressionSymbol symbol)
    {
        switch (symbol.Operator)
        {
            //*
            //*
            //* arithmentic expressions.
            //*
            case Symbols.OperatorType.Add:
                return VisitAddExpr(context, (AddExprSymbol)symbol);

            case Symbols.OperatorType.Subtract:
                return VisitSubtractExpr(context, (SubtractExprSymbol)symbol);

            case Symbols.OperatorType.Divide:
                break;
            case Symbols.OperatorType.Multiply:
                break;
            case Symbols.OperatorType.Modulo:
                break;

            //*
            //*
            //* relational expressions.
            //*
            case Symbols.OperatorType.Equals:
                break;
            case Symbols.OperatorType.NotEquals:
                break;
            case Symbols.OperatorType.Less:
                break;
            case Symbols.OperatorType.LessEquals:
                break;
            case Symbols.OperatorType.Greater:
                break;
            case Symbols.OperatorType.GreaterEquals:
                break;

            //*
            //*
            //* pattern match expressions.
            //*
            case Symbols.OperatorType.Like:
                break;
            case Symbols.OperatorType.RegexMatch:
                break;

            //*
            //*
            //* logical expressions.
            //*
            case Symbols.OperatorType.Or:
                break;
            case Symbols.OperatorType.And:
                break;
            case Symbols.OperatorType.Not:
                break;

            //*
            //*
            //* semantic expressions.
            //*
            case Symbols.OperatorType.Expr:
                break;
            case Symbols.OperatorType.Parse:
                break;
            case Symbols.OperatorType.Select:
                break;
            case Symbols.OperatorType.Type:
                break;
            case Symbols.OperatorType.MemberAccess:
                return VisitMemberAccess(context, (MemberAccessExprSymbol)symbol);

            //*
            //*
            //* query expressions.
            //*
            case Symbols.OperatorType.Filter:
                return VisitFilterExpr(context, (FilterExprSymbol)symbol);

            case Symbols.OperatorType.Project:
                break;
            case Symbols.OperatorType.Transform:
                break;
            case Symbols.OperatorType.SelectMany:
                break;
            case Symbols.OperatorType.Limit:
                break;
            case Symbols.OperatorType.Skip:
                break;

            //*
            //*
            //* aggregation expressions.
            //*
            case Symbols.OperatorType.Count:
                break;
            case Symbols.OperatorType.Index:
                break;
            case Symbols.OperatorType.Any:
                break;
            case Symbols.OperatorType.All:
                break;
            case Symbols.OperatorType.Min:
                break;
            case Symbols.OperatorType.Max:
                break;
            case Symbols.OperatorType.Sum:
                break;
            case Symbols.OperatorType.Average:
                break;

            default:
                break;
        }

        throw new Exception();
    }

    //*
    //*
    //* arithmetic expressions.
    //*
    protected virtual ExpressionSymbol VisitAddExpr(SemanticContext context, AddExprSymbol symbol)
    {
        return new AddExprSymbol(
            destination: VisitDestination(context, symbol.Destination),
            left: VisitExpression(context, symbol.LeftOperand),
            right: VisitExpression(context, symbol.RightOperand)
        );
    }

    protected virtual ExpressionSymbol VisitSubtractExpr(SemanticContext context, SubtractExprSymbol symbol)
    {
        return new SubtractExprSymbol(
            destination: VisitDestination(context, symbol.Destination),
            left: VisitExpression(context, symbol.LeftOperand),
            right: VisitExpression(context, symbol.RightOperand)
        );
    }

    //*
    //*
    //* semantic expressions.
    //*
    public virtual ExpressionSymbol VisitMemberAccess(SemanticContext context, MemberAccessExprSymbol symbol)
    {
        return new MemberAccessExprSymbol(
            operand: VisitExpression(context, symbol.Operand),
            memberName: symbol.MemberName
        );
    }

    //*
    //*
    //* query expressions.
    //*
    protected virtual ExpressionSymbol VisitFilterExpr(SemanticContext context, FilterExprSymbol symbol)
    {
        return new FilterExprSymbol(
           destination: VisitDestination(context, symbol.Destination),
           source: VisitExpression(context, symbol.Source),
           lambda: VisitLambda(context, symbol.Lambda)
        );
    }

    protected virtual DestinationSymbol VisitDestination(SemanticContext context, DestinationSymbol symbol)
    {
        return symbol;
    }

}
