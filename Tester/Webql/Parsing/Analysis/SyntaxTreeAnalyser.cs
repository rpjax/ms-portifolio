using Webql.Parsing.Ast;
using Webql.Parsing.Components;

namespace Webql.Parsing.Analysis;

/// <summary>
/// Represents a visitor for analyzing the syntax tree.
/// </summary>
public class SyntaxTreeAnalyzer : SyntaxTreeVisitor
{
    /// <summary>
    /// Executes the analysis of the syntax tree.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree to analyze.</param>
    public void ExecuteAnalysis(WebqlSyntaxNode? syntaxTree)
    {
        Analyze(syntaxTree);
    }

    /// <summary>
    /// Analyzes the given syntax tree node.
    /// </summary>
    /// <param name="node">The syntax tree node to analyze.</param>
    protected virtual void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                AnalyzeQuery((WebqlQuery)node);
                return;

            case WebqlNodeType.Expression:
                AnalyzeExpression((WebqlExpression)node);
                return;

            default:
                throw new InvalidOperationException("Invalid node type.");
        }
    }

    /// <summary>
    /// Analyzes the given query.
    /// </summary>
    /// <param name="query">The query to analyze.</param>
    protected virtual void AnalyzeQuery(WebqlQuery query)
    {
        Analyze(query.Expression);
    }

    /// <summary>
    /// Analyzes the given expression.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    protected virtual void AnalyzeExpression(WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                AnalyzeLiteralExpression((WebqlLiteralExpression)expression);
                return;

            case WebqlExpressionType.Reference:
                AnalyzeReferenceExpression((WebqlReferenceExpression)expression);
                return;

            case WebqlExpressionType.ScopeAccess:
                AnalyzeScopeAccessExpression((WebqlScopeAccessExpression)expression);
                return;

            case WebqlExpressionType.TemporaryDeclaration:
                AnalyzeTemporaryDeclarationExpression((WebqlTemporaryDeclarationExpression)expression);
                return;

            case WebqlExpressionType.Block:
                AnalyzeBlockExpression((WebqlBlockExpression)expression);
                return;

            case WebqlExpressionType.Operation:
                AnalyzeOperationExpression((WebqlOperationExpression)expression);
                return;

            default:
                throw new InvalidOperationException("Invalid operand type.");
        }
    }

    /// <summary>
    /// Analyzes the given literal expression.
    /// </summary>
    /// <param name="literalExpression">The literal expression to analyze.</param>
    protected virtual void AnalyzeLiteralExpression(WebqlLiteralExpression literalExpression)
    {
        return;
    }

    /// <summary>
    /// Analyzes the given reference expression.
    /// </summary>
    /// <param name="referenceExpression">The reference expression to analyze.</param>
    protected virtual void AnalyzeReferenceExpression(WebqlReferenceExpression referenceExpression)
    {
        return;
    }

    /// <summary>
    /// Analyzes the given scope access expression.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression to analyze.</param>
    protected virtual void AnalyzeScopeAccessExpression(WebqlScopeAccessExpression scopeAccessExpression)
    {
        Analyze(scopeAccessExpression.Expression);
    }

    /// <summary>
    /// Analyzes the given temporary declaration expression.
    /// </summary>
    /// <param name="temporaryDeclarationExpression">The temporary declaration expression to analyze.</param>
    protected virtual void AnalyzeTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        Analyze(temporaryDeclarationExpression.Value);
    }

    /// <summary>
    /// Analyzes the given block expression.
    /// </summary>
    /// <param name="blockExpression">The block expression to analyze.</param>
    protected virtual void AnalyzeBlockExpression(WebqlBlockExpression blockExpression)
    {
        foreach (var expression in blockExpression.Expressions)
        {
            Analyze(expression);
        }
    }

    /// <summary>
    /// Analyzes the given operation expression.
    /// </summary>
    /// <param name="operationExpression">The operation expression to analyze.</param>
    protected virtual void AnalyzeOperationExpression(WebqlOperationExpression operationExpression)
    {
        foreach (var operand in operationExpression.Operands)
        {
            Analyze(operand);
        }
    }

}
