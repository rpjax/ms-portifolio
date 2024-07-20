using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ModularSystem.Core.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Webql.Core.Extensions;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Core;

public class WebqlCompilerSettings
{
    public WebqlLinqProvider LinqProvider { get; }
    public ISyntaxTreeVisitor[] PreValidationVisitors { get; }
    public ISyntaxTreeVisitor[] PostValidationVisitors { get; }

    public WebqlCompilerSettings(
        WebqlLinqProvider? linqProvider = null,
        ISyntaxTreeVisitor[]? preValidationVisitors = null,
        ISyntaxTreeVisitor[]? postValidationVisitors = null)
    {
        LinqProvider = linqProvider ?? new WebqlLinqProvider();
        PreValidationVisitors = preValidationVisitors ?? new ISyntaxTreeVisitor[0];
        PostValidationVisitors = postValidationVisitors ?? new ISyntaxTreeVisitor[0];
    }

}

public class WebqlLinqProvider
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

        if(sourceType.IsAsyncQueryable())
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

    public MethodInfo GetTakeMethodInfo(Type sourceType)
    {
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

    public MethodInfo GetSkipMethodInfo(Type sourceType)
    {
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

    public MethodInfo GetCountMethodInfo(Type sourceType)
    {
        var elementType = sourceType.GetQueryableElementType();
        throw new NotImplementedException();
    }

    public MethodInfo GetContainsMethodInfo(Type sourceType)
    {
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Enumerable).GetMethods()
            .Where(x => x.Name == "Contains")
            .Where(x => x.GetParameters().Length == 2)
            .First()
            .MakeGenericMethod(elementType);
    }

    public MethodInfo GetIndexMethodInfo(Type sourceType)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAnyMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAllMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetMinMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetMaxMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetSumMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }

    public MethodInfo GetAverageMethodInfo(Type type)
    {
        throw new NotImplementedException();
    }
}
