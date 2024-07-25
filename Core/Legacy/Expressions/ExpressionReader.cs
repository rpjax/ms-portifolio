using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

/// <summary>
/// *under development.*
/// </summary>
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

/// <summary>
/// *under development.*
/// </summary>
public class SelectorWriter
{
    private Expression? Expression { get; set; }

    public SelectorWriter SetSelector<TEntity, TField>()
    {
        return this;
    }
}