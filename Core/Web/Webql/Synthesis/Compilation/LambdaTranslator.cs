using ModularSystem.Webql.Synthesis.Symbols;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation;

public class LambdaArgumentTranslator
{
    public ParameterExpression TranslateLambdaArgument(TranslationContext context, LambdaArgument arg, Type type)
    {
        if (string.IsNullOrEmpty(arg.Value))
        {
            throw new TranslationException("", context);
        }

        return Expression.Parameter(type, arg.Value);
    }

    public ParameterExpression[] TranslateLambdaArguments(TranslationContext context, LambdaArguments args, Type[] types)
    {
        var expressions = new List<ParameterExpression>();

        if (args.Arguments.Length != types.Length)
        {
            throw new TranslationException("", context);
        }
        
        for (int i = 0; i < args.Arguments.Length; i++)
        {
            var arg = args.Arguments[i];
            var type = types[i];

            expressions.Add(TranslateLambdaArgument(context, arg, type));
        }

        return expressions.ToArray();
    }
}


public class LambdaTranslator
{
    public LambdaExpression TranslateLambda(TranslationContext context, Lambda unaryLambda, Type[] paramTypes)
    {
        var argExpr = new LambdaArgumentTranslator()
            .TranslateLambdaArguments(context, unaryLambda.Arguments, paramTypes);

        var bodyExpr = new ObjectTranslator()
            .TranslateObject(context, unaryLambda.Body);

        return Expression.Lambda(bodyExpr, argExpr);
    }

    public LambdaExpression TranslateUnaryLambda(TranslationContext context, UnaryLambda unaryLambda, Type paramType)
    {
        var argExpr = new LambdaArgumentTranslator()
            .TranslateLambdaArgument(context, unaryLambda.Argument, paramType);

        var bodyExpr = new ObjectTranslator()
            .TranslateObject(context, unaryLambda.Body);

        return Expression.Lambda(bodyExpr, argExpr);
    }

}

public class ObjectTranslator
{
    public Expression TranslateObject(TranslationContext context, Symbols.Object obj)
    {
        var expr = null as Expression;
        var translator = new ExprTranslator();

        foreach (var item in obj)
        {
            expr = translator.TranslateExpr(context, item);
        }

        if(expr is null)
        {
            throw new TranslationException("", context);
        }

        return expr;
    }
}

public class ReferenceTranslator
{
    public Expression TranslateReference(TranslationContext context, Reference arg)
    {
        return context.GetSymbol(arg.Value).Expression;
    }
}

public class ArgumentTranslator
{
    public Expression TranslateArgument(TranslationContext context, Argument arg)
    {
        if(arg is Reference reference)
        {
            return new ReferenceTranslator()
                .TranslateReference(context, reference);
        }

        if(arg is Symbols.Object obj)
        {
            return new ObjectTranslator()
                .TranslateObject(context, obj); 
        }

        throw new TranslationException("", context);
    }

    public QueryArgumentExpression TranslateQueryArgument(TranslationContext context, Argument arg)
    {
        return new QueryArgumentExpression(TranslateQueryArgument(context, arg));
    }
}

public class ExprTranslator
{
    public Expression TranslateExpr(TranslationContext context, Expr expr)
    {
        //*
        // arithmetic ops.
        //*

        if (expr is AddExpr addExpr)
        {
            return new ArithmeticExprTranslator()
                .TranslateAddExpr(context, addExpr);
        }

        //*
        // pattern match ops.
        //*

        if(expr is LikeExpr likeExpr)
        {
            return new PatternRelationalExprTranslator()
                .TranslateLikeExpr(context, likeExpr);
        }

        throw new TranslationException("", context);
    }
}

public class DestinationTranslator
{
    public string? TranslateDestination(TranslationContext context, Destination destination)
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
    public BinaryArgumentsExpression TranslateBinaryArguments(TranslationContext context, BinaryArguments arguments)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, arguments.Destination);

        var left = new ArgumentTranslator()
            .TranslateArgument(context, arguments.Left);

        var right = new ArgumentTranslator()
          .TranslateArgument(context, arguments.Right);

        return new BinaryArgumentsExpression(
            destination: destination,
            left: left,
            right: right
        );
    }
}

public class ArithmeticExprTranslator
{
    public Expression TranslateAddExpr(TranslationContext context, AddExpr addExpr)
    {
        var binaryArgsExpr = new BinaryArgumentsTranslator()
            .TranslateBinaryArguments(context, addExpr.Arguments);

        var destination = binaryArgsExpr.Destination;
        var left = binaryArgsExpr.Left; 
        var right = binaryArgsExpr.Right;

        var expr = Expression.Add(left, right); 

        if(destination is not null)
        {
            context.SetSymbol(destination, expr);
        }

        return expr;
    }
}

public class PatternRelationalExprTranslator
{
    public Expression TranslateLikeExpr(TranslationContext context, LikeExpr likeExpr)
    {
        var argsExpr = new BinaryArgumentsTranslator()
            .TranslateBinaryArguments(context, likeExpr.Arguments);

        var destination = argsExpr.Destination;
        var left = argsExpr.Left;
        var right = argsExpr.Right;

        var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { })!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        var lhsTolowerExpression = Expression.Call(left, toLowerMethod);
        var rhsToLowerExpression = Expression.Call(right, toLowerMethod);

        var containsArgs = new Expression[] { rhsToLowerExpression };

        var expr = Expression.Call(lhsTolowerExpression, containsMethod, containsArgs);

        if (destination is not null)
        {
            context.SetSymbol(destination, expr);
        }

        return expr;
    }

}

public class QueryExprTranslator
{
    public Expression TranslateFilterExpr(TranslationContext context, FilterExpr filterExpr)
    {
        var destination = new DestinationTranslator()
            .TranslateDestination(context, filterExpr.Destination);

        var source = new ArgumentTranslator()
            .TranslateQueryArgument(context, filterExpr.Source);

        var lambda = new LambdaTranslator()
            .TranslateUnaryLambda(context, filterExpr.Lambda, source.GetElementType(context));

        var methodInfo = new TranslationOptions().LinqProvider
           .GetWhereMethodInfo(context, source);

        var expr = Expression.Call(methodInfo, source.Expression, lambda);

        if (destination is not null)
        {
            context.SetSymbol(destination, lambda);
        }

        return expr;
    }
}
