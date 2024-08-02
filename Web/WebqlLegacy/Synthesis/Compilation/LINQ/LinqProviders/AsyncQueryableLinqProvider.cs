using Aidan.Core.Linq;
using System.Reflection;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Provides a LINQ provider for asynchronous queryable data sources that implements the IAsyncQueryable interface. <br/>
/// This class provides the necessary method info for asynchronous query operations on IAsyncQueryable data sources.
/// </summary>
public class AsyncQueryableLinqProvider : LinqProvider
{
    /// <summary>
    /// Retrieves MethodInfo for the 'Where' method in the AsyncQueryableExtensions class.
    /// </summary>
    /// <returns>MethodInfo for 'Where' method.</returns>
    protected override MethodInfo GetQueryableWhereMethodInfo()
    {
        return typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "Where")
            .First();
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Select' method in the AsyncQueryableExtensions class.
    /// </summary>
    /// <returns>MethodInfo for 'Select' method.</returns>
    protected override MethodInfo GetQueryableSelectMethodInfo()
    {
        return typeof(AsyncQueryableExtensions)
            .GetMethods()
            .Where(m => m.Name == "Select")
            .First();
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'SelectMany' method in the AsyncQueryableExtensions class.
    /// </summary>
    /// <returns>MethodInfo for 'SelectMany' method.</returns>
    protected override MethodInfo GetQueryableSelectManyMethodInfo()
    {
        return typeof(AsyncQueryableExtensions)
            .GetMethods()
            .Where(m => m.Name == "SelectMany")
            .First();
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Take' method in the AsyncQueryableExtensions class.
    /// </summary>
    /// <returns>MethodInfo for 'Take' method.</returns>
    protected override MethodInfo GetQueryableTakeMethodInfo()
    {
        return typeof(AsyncQueryableExtensions)
           .GetMethods()
           .Where(m => m.Name == "Take")
           .First();
    }

    /// <summary>
    /// Retrieves MethodInfo for the 'Skip' method in the AsyncQueryableExtensions class.
    /// </summary>
    /// <returns>MethodInfo for 'Skip' method.</returns>
    protected override MethodInfo GetQueryableSkipMethodInfo()
    {
        return typeof(AsyncQueryableExtensions)
           .GetMethods()
           .Where(m => m.Name == "Skip")
           .First();
    }
}
