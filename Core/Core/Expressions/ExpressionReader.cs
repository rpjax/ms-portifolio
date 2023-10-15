using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

public class ExpressionReader
{
    private Expression Expression { get; }

    public ExpressionReader(Expression expression)
    {
        Expression = expression;
    }

    public Expression<Func<T, bool>> GetPredicate<T>()
    {
        //*
        // todo: add validation logic
        //*
        return Expression.TypeCast<Expression<Func<T, bool>>>();
    }
}

public class SelectorExpressionWriter
{
    private Expression? Expression { get; set; }

    public 
}