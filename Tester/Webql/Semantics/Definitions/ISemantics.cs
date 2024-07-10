namespace Webql.Semantics.Definitions;

/*
 * Semantics
 */

public interface ISemantics
{

}

public interface ITypedSemantics : ISemantics
{
    Type Type { get; }
}

public interface IQuerySemantics : ITypedSemantics
{
    
}

public interface IExpressionSemantics : ITypedSemantics
{
    
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

public class ExpressionSemantics : IExpressionSemantics
{
    public Type Type { get; }

    public ExpressionSemantics(Type type)
    {
        Type = type;
    }
}
