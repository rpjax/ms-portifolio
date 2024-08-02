using System.Linq.Expressions;
using System.Reflection;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Provides a LINQ provider for queryable data sources that implements the IQueryable interface.
/// This class provides the necessary method info for standard query operations on IQueryable data sources.
/// </summary>
public class QueryableLinqProvider : LinqProvider
{
    /// <summary>
    /// Retrieves MethodInfo for the 'Where' method in the Queryable class.
    /// </summary>
    /// <returns>MethodInfo for 'Where' method.</returns>
    protected override MethodInfo GetQueryableWhereMethodInfo()
    {
        return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
           .First(m => m.Name == "Where" &&
                   m.IsGenericMethodDefinition &&
                   m.GetParameters().Length == 2 &&
                   m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                   m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Select' method in the Queryable class.
    /// </summary>
    /// <returns>MethodInfo for 'Select' method.</returns>
    protected override MethodInfo GetQueryableSelectMethodInfo()
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
           .First(m => m != null);
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'SelectMany' method in the Queryable class.
    /// </summary>
    /// <returns>MethodInfo for 'SelectMany' method.</returns>
    protected override MethodInfo GetQueryableSelectManyMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == "SelectMany" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length >= 2
                && x.Args.Length == 2
                && x.Params[0].ParameterType.IsGenericType
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                && x.Params[1].ParameterType.IsGenericType
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                && x.Params[1].ParameterType.GetGenericArguments()[0].IsGenericType
                && x.Params[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>))
            .Select(x => x.Method)
            .First(m => m.GetParameters().Length == 2 && m != null);
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Take' method in the Queryable class.
    /// </summary>
    /// <returns>MethodInfo for 'Take' method.</returns>
    protected override MethodInfo GetQueryableTakeMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Take" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Skip' method in the Queryable class.
    /// </summary>
    /// <returns>MethodInfo for 'Skip' method.</returns>
    protected override MethodInfo GetQueryableSkipMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Skip" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType == typeof(int));
    }

}
