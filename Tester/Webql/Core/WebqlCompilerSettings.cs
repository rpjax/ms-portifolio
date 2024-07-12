using ModularSystem.Core.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Core;

public class WebqlCompilerSettings
{
    public static Type DefaultQueryableType { get; } = typeof(IQueryable<>);
    public static Type DefaultAsyncQueryableType { get; } = typeof(IAsyncQueryable<>);

    public Type QueryableType { get; } 
    public Type ElementType { get; }
    public MethodInfoProvider MethodInfoProvider { get; }

    public WebqlCompilerSettings(Type queryableType, Type entityType, MethodInfoProvider methodInfoProvider)
    {
        QueryableType = queryableType;
        ElementType = entityType;
        MethodInfoProvider = methodInfoProvider;
    }

}

public class MethodInfoProvider
{
    public MethodInfo GetWhereMethodInfo(Type type)
    {
        var elementType = type.GetQueryableElementType();

        if(type.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions).GetMethods()
                .Where(x => x.Name == "Where")
                .First()
                .MakeGenericMethod(elementType);
                ;
        }
        else
        {
            return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .MakeGenericMethod(elementType);
                ;
        }
    }
}
