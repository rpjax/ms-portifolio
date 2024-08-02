using Aidan.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast.Builder;

/// <summary>
/// Represents the context for building the Webql Abstract Syntax Tree (AST).
/// </summary>
public class WebqlAstBuildContext
{
    /// <summary>
    /// Gets the scope type of the context.
    /// </summary>
    public WebqlScopeType ScopeType { get; private set; }

    private WebqlExpression LhsExpression { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlAstBuildContext"/> class.
    /// </summary>
    /// <param name="scopeType">The scope type of the context.</param>
    /// <param name="lhsExpression">The left-hand side expression.</param>
    public WebqlAstBuildContext(
        WebqlScopeType scopeType,
        WebqlExpression lhsExpression)
    {
        ScopeType = scopeType;
        LhsExpression = lhsExpression;
    }

    /// <summary>
    /// Creates a child context based on the current context.
    /// </summary>
    /// <returns>A new instance of the <see cref="WebqlAstBuildContext"/> class.</returns>
    public WebqlAstBuildContext CreateChildContext()
    {
        return new WebqlAstBuildContext(
            scopeType: ScopeType,
            lhsExpression: LhsExpression
        );
    }

    /// <summary>
    /// Gets the left-hand side expression.
    /// </summary>
    /// <param name="node">The CstNode.</param>
    /// <returns>The left-hand side expression.</returns>
    public WebqlExpression GetLhsExpression(CstNode node)
    {
        return LhsExpression;
    }

    /// <summary>
    /// Sets the left-hand side expression.
    /// </summary>
    /// <param name="lhsExpression">The left-hand side expression.</param>
    public void SetLhsExpression(WebqlExpression lhsExpression)
    {
        LhsExpression = lhsExpression;
    }

    /// <summary>
    /// Sets the scope type of the context.
    /// </summary>
    /// <param name="scopeType">The scope type of the context.</param>
    public void SetScopeType(WebqlScopeType scopeType)
    {
        ScopeType = scopeType;
    }
}
