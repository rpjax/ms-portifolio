using System.Reflection;
using Webql.Parsing.Ast;

namespace Webql.Core.Linq;

/// <summary>
/// Represents a provider for generating LINQ expressions based on Webql expressions.
/// </summary>
public interface IWebqlLinqProvider
{
    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "All" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "All" method.</returns>
    MethodInfo GetAllMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Any" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Any" method.</returns>
    MethodInfo GetAnyMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Average" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Average" method.</returns>
    MethodInfo GetAverageMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Contains" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Contains" method.</returns>
    MethodInfo GetContainsMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Count" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Count" method.</returns>
    MethodInfo GetCountMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Index" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Index" method.</returns>
    MethodInfo GetIndexMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Max" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Max" method.</returns>
    MethodInfo GetMaxMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Min" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Min" method.</returns>
    MethodInfo GetMinMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="Type"/> of the queryable based on the given <see cref="WebqlSyntaxNode"/>.
    /// </summary>
    /// <param name="node">The <see cref="WebqlSyntaxNode"/>.</param>
    /// <returns>The <see cref="Type"/> of the queryable.</returns>
    Type GetQueryableType(WebqlSyntaxNode node);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Select" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <param name="selector">The selector <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Select" method.</returns>
    MethodInfo GetSelectMethodInfo(WebqlExpression source, WebqlExpression selector);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Skip" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Skip" method.</returns>
    MethodInfo GetSkipMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Sum" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Sum" method.</returns>
    MethodInfo GetSumMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Take" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Take" method.</returns>
    MethodInfo GetTakeMethodInfo(WebqlExpression source);

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> for the "Where" method.
    /// </summary>
    /// <param name="source">The source <see cref="WebqlExpression"/>.</param>
    /// <returns>The <see cref="MethodInfo"/> for the "Where" method.</returns>
    MethodInfo GetWhereMethodInfo(WebqlExpression source);
}
