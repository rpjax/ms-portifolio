using Aidan.Core.Reflection;
using System.Reflection;

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

public interface IMemberAccessSemantics : IExpressionSemantics
{
    PropertyInfo PropertyInfo { get; }
}

public interface IAnonymousObjectPropertySemantics : ITypedSemantics
{
    string Name { get; }
    PropertyInfo PropertyInfo { get; }
}

public interface IAnonymousObjectSemantics : IExpressionSemantics
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

public class MemberAccessSemantics : IMemberAccessSemantics
{
    public Type Type { get; }
    public PropertyInfo PropertyInfo { get; }

    public MemberAccessSemantics(Type type, PropertyInfo propertyInfo)
    {
        Type = type;
        PropertyInfo = propertyInfo;
    }
}

public class AnonymousObjectPropertySemantics : IAnonymousObjectPropertySemantics
{
    public string Name { get; }
    public Type Type { get; }
    public PropertyInfo PropertyInfo { get; }

    public AnonymousObjectPropertySemantics(string name, Type type, PropertyInfo propertyInfo)
    {
        Name = name;
        Type = type;
        PropertyInfo = propertyInfo;
    }
}

public class AnonymousObjectSemantics : IAnonymousObjectSemantics
{
    public Type Type { get; }

    public AnonymousObjectSemantics(Type type)
    {
        Type = type;
    }
}
