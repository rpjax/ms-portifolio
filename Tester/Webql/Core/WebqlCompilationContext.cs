using ModularSystem.Core;
using Webql.Core.Linq;
using Webql.Parsing.Ast;

namespace Webql.Core;

/// <summary>
/// Represents the compilation context for a query.
/// </summary>
public class WebqlCompilationContext
{
    /// <summary>
    /// Gets the Webql compiler settings.
    /// </summary>
    public WebqlCompilerSettings Settings { get; }

    /// <summary>
    /// Gets the type of the element.
    /// </summary>
    public Type ElementType { get; }

    private List<Error> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlCompilationContext"/> class.
    /// </summary>
    /// <param name="settings">The Webql compiler settings.</param>
    /// <param name="elementType">The type of the element.</param>
    public WebqlCompilationContext(
        WebqlCompilerSettings settings,
        Type elementType)
    {
        Settings = settings;
        ElementType = elementType;
        Errors = new List<Error>();
    }

    /// <summary>
    /// Gets the Webql LINQ provider.
    /// </summary>
    public IWebqlLinqProvider LinqProvider => Settings.LinqProvider;

    /// <summary>
    /// Gets the queryable type for the specified Webql syntax node.
    /// </summary>
    /// <param name="node">The Webql syntax node.</param>
    /// <returns>The queryable type.</returns>
    public Type GetQueryableType(WebqlSyntaxNode node)
    {
        return LinqProvider.GetQueryableType(node);
    }
}
