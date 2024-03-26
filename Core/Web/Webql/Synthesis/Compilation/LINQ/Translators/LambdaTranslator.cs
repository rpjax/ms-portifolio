using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class LambdaTranslator
{
    public LambdaExpression TranslateLambda(TranslationContext context, LambdaSymbol symbol)
    {
        var paramExprs = new LambdaArgumentTranslator()
            .TranslateLambdaArguments(context, symbol.Arguments);

        var bodyExpr = new StatementBlockTranslator()
            .TranslateStatementBlock(context, symbol.Body);

        return Expression.Lambda(bodyExpr, paramExprs);
    }

}

public class ExprTranslator
{
    public Expression TranslateExpr(TranslationContext context, OperatorExpressionSymbol expr)
    {
        //*
        // arithmetic ops.
        //*

        if (expr is AddExprSymbol addExpr)
        {
            return new ArithmeticExprTranslator()
                .TranslateAddExpr(context, addExpr);
        }

        //*
        // pattern match ops.
        //*

        if(expr is LikeExprSymbol likeExpr)
        {
            return new PatternRelationalExprTranslator()
                .TranslateLikeExpr(context, likeExpr);
        }

        throw new Exception();
    }
}

public class DestinationTranslator
{
    public string? TranslateDestination(TranslationContext context, DestinationSymbol destination)
    {
        return destination.Value;
    }
}

public class BinaryArgumentsExpression
{
    public string? Destination { get; }
    public Expression Left { get; }
    public Expression Right { get; }

    public BinaryArgumentsExpression(string? destination, Expression left, Expression right)
    {
        Destination = destination;
        Left = left;
        Right = right;
    }
}

public class BinaryArgumentsTranslator
{
    public BinaryArgumentsExpression TranslateBinaryArguments(TranslationContext context, BinaryArgumentsSymbol symbol)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, symbol.Destination);

        var left = new ExpressionTranslator()
            .TranslateExpression(context, symbol.LeftOperand);

        var right = new ExpressionTranslator()
            .TranslateExpression(context, symbol.RightOperand);

        return new BinaryArgumentsExpression(
            destination: destination,
            left: left,
            right: right
        );
    }
}

public class ArithmeticExprTranslator
{
    public Expression TranslateAddExpr(TranslationContext context, AddExprSymbol addExpr)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, addExpr.Destination);

        var left = new ExpressionTranslator()
            .TranslateExpression (context, addExpr.LeftOperand);

        var right = new ExpressionTranslator()
            .TranslateExpression(context, addExpr.RightOperand);

        var expr = Expression.Add(left, right); 

        if(destination is not null)
        {
            context.CreateOrUpdateTranslationTableEntry(destination, expr);
        }

        return expr;
    }
}

public class PatternRelationalExprTranslator
{
    public Expression TranslateLikeExpr(TranslationContext context, LikeExprSymbol likeExpr)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, likeExpr.Destination);

        var left = new ExpressionTranslator()
            .TranslateExpression(context, likeExpr.LeftOperand);

        var right = new ExpressionTranslator()
            .TranslateExpression(context, likeExpr.RightOperand);

        var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { })!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        var lhsTolowerExpression = Expression.Call(left, toLowerMethod);
        var rhsToLowerExpression = Expression.Call(right, toLowerMethod);

        var containsArgs = new Expression[] { rhsToLowerExpression };

        var expr = Expression.Call(lhsTolowerExpression, containsMethod, containsArgs);

        if (destination is not null)
        {
            context.CreateOrUpdateTranslationTableEntry(destination, expr);
        }

        return expr;
    }

}

public class QueryExprTranslator
{
    public Expression TranslateFilterExpr(TranslationContext context, FilterExprSymbol filterExpr)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, filterExpr.Destination);

        var source = new ExpressionTranslator()
            .TranslateExpression(context, filterExpr.Source);

        var lambda = new LambdaTranslator()
            .TranslateLambda(context, filterExpr.Lambda);

        var sourceSemantics = filterExpr.Source
            .GetSemantics<ArgumentSemantic>(context);

        var methodInfo = new TranslationOptions().LinqProvider
           .GetWhereMethodInfo(context, sourceSemantics);

        var expr = Expression.Call(methodInfo, source, lambda);

        if (destination is not null)
        {
            context.CreateOrUpdateTranslationTableEntry(destination, expr, SymbolAccessMode.ReadWrite);
        }

        return expr;
    }
}
