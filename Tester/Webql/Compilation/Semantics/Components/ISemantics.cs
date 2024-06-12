namespace Webql.Semantics.Components;

/*
 * Semantics
 */

public interface ISemantics
{

}

public interface IQuerySemantics : ISemantics
{
    Type Type { get; }  
}

public interface IExpressionSemantics : ISemantics
{
    Type Type { get; }
}

/*
 * Concrete implementations
 */

public class QuerySemantics : IQuerySemantics
{
    public Type Type { get; }

    public QuerySemantics(Type type)
    {
        Type = type;
    }
}

public class LhsSemantics : IExpressionSemantics
{
    public Type Type { get; }

    public LhsSemantics(Type type)
    {
        Type = type;
    }
}

public class ExpressionSemantics : IExpressionSemantics
{
    public Type Type { get; }

    public ExpressionSemantics(Type type)
    {
        Type = type;
    }
}
