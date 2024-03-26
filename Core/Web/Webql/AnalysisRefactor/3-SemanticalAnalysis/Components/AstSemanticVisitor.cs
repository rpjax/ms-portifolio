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
        if (symbol is LambdaSymbol lambdaSymbol)
        {
            return VisitLambda(context, lambdaSymbol);
        }
        if (symbol is LambdaArgumentSymbol lambdaArgument)
        {
            return VisitLambdaArgument(context, lambdaArgument);
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

    protected virtual LambdaSymbol VisitLambda(SemanticContext context, LambdaSymbol symbol)
    {
        return new LambdaSymbol(
            arguments: symbol.Arguments
                .Select(x => VisitLambdaArgument(context, x))
                .ToArray(),
            body: VisitStatementBlock(context, symbol.Body)
        );
    }

    protected virtual LambdaArgumentSymbol VisitLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
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
            case ExpressionOperator.Add:
                return VisitAddExpr(context, (AddExprSymbol)symbol);

            case ExpressionOperator.Subtract:
                return VisitSubtractExpr(context, (SubtractExprSymbol)symbol);

            case ExpressionOperator.Divide:
                break;
            case ExpressionOperator.Multiply:
                break;
            case ExpressionOperator.Modulo:
                break;
            case ExpressionOperator.Equals:
                break;
            case ExpressionOperator.NotEquals:
                break;
            case ExpressionOperator.Less:
                break;
            case ExpressionOperator.LessEquals:
                break;
            case ExpressionOperator.Greater:
                break;
            case ExpressionOperator.GreaterEquals:
                break;
            case ExpressionOperator.Like:
                break;
            case ExpressionOperator.RegexMatch:
                break;
            case ExpressionOperator.Or:
                break;
            case ExpressionOperator.And:
                break;
            case ExpressionOperator.Not:
                break;
            case ExpressionOperator.Expr:
                break;
            case ExpressionOperator.Parse:
                break;
            case ExpressionOperator.Select:
                break;
            case ExpressionOperator.Filter:
                break;
            case ExpressionOperator.Project:
                break;
            case ExpressionOperator.Transform:
                break;
            case ExpressionOperator.SelectMany:
                break;
            case ExpressionOperator.Limit:
                break;
            case ExpressionOperator.Skip:
                break;
            case ExpressionOperator.Count:
                break;
            case ExpressionOperator.Index:
                break;
            case ExpressionOperator.Any:
                break;
            case ExpressionOperator.All:
                break;
            case ExpressionOperator.Min:
                break;
            case ExpressionOperator.Max:
                break;
            case ExpressionOperator.Sum:
                break;
            case ExpressionOperator.Average:
                break;

            default:
                break;
        }

        throw new Exception();
    }

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

    protected virtual DestinationSymbol VisitDestination(SemanticContext context, DestinationSymbol symbol)
    {
        return symbol;
    }

}
