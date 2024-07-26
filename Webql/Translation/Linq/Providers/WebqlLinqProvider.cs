using ModularSystem.Core.Linq;
using ModularSystem.Core.Linq.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using Webql.Core;
using Webql.Core.Extensions;
using Webql.Core.Linq;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Translation.Linq.Providers;

public class WebqlLinqProvider : IWebqlLinqProvider
{
    public static Type DefaultQueryableType { get; } = typeof(IQueryable<>);
    public static Type DefaultAsyncQueryableType { get; } = typeof(IAsyncQueryable<>);

    /*
     * MethodInfo cache
     */

    // TODO: Implement a cache for MethodInfo objects

    /*
     * Type providers
     */

    public Type GetQueryableType(WebqlSyntaxNode node)
    {
        return typeof(IQueryable<>);
    }

    /*
     * Collection Manipulation LINQ methods
     */

    public MethodInfo GetWhereMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
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

    public MethodInfo GetSelectMethodInfo(WebqlExpression source, WebqlExpression selector)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();
        var resultType = selector.GetExpressionType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Select")
                .First()
                .MakeGenericMethod(elementType, resultType);
            ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                    && x.Args.Length == 2
                    && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                    && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .Select(x => x.Method)
                .First(m => m != null)
                .MakeGenericMethod(elementType, resultType);
            ;
        }
    }

    public MethodInfo GetTakeMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Take")
                .First()
                .MakeGenericMethod(elementType)
                ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Take" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType == typeof(int))
                .MakeGenericMethod(elementType);
            ;
        }
    }

    public MethodInfo GetSkipMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Skip")
                .First()
                .MakeGenericMethod(elementType)
                ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Skip" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType == typeof(int))
                .MakeGenericMethod(elementType);
            ;
        }
    }

    /*
     * Collection Aggregation LINQ methods
     */

    public MethodInfo GetCountMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();
        throw new NotImplementedException();
    }

    public MethodInfo GetContainsMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Enumerable).GetMethods()
            .Where(x => x.Name == "Contains")
            .Where(x => x.GetParameters().Length == 2)
            .First()
            .MakeGenericMethod(elementType);
    }

    public MethodInfo GetIndexMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAnyMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAllMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetMinMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetMaxMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetSumMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAverageMethodInfo(WebqlExpression source)
    {
        throw new NotImplementedException();
    }
}
